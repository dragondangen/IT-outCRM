using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;

namespace IT_outCRM.Blazor.Services
{
    public class TokenStorage : ITokenStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string TokenKey = "authToken";
        private string? _cachedToken; // In-memory cache (per instance, Scoped)
        private static string? _sharedToken; // Shared token across all instances (for HttpClient)
        private static readonly object _lock = new object();
        private readonly ILogger<TokenStorage> _logger;

        public TokenStorage(
            IJSRuntime jsRuntime,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TokenStorage> logger)
        {
            _jsRuntime = jsRuntime;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            Console.WriteLine($"[TokenStorage] Created instance {this.GetHashCode()}");
        }

        public async Task<string?> GetTokenAsync()
        {
            Console.WriteLine($"[TokenStorage {this.GetHashCode()}] GetTokenAsync called. Cached: {!string.IsNullOrEmpty(_cachedToken)}, Shared: {!string.IsNullOrEmpty(_sharedToken)}");
            
            // 1. Проверяем кэш экземпляра
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                Console.WriteLine($"[TokenStorage] Returning cached token");
                return _cachedToken;
            }
            
            // 1.5. Проверяем общий кэш (для HttpClient запросов)
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(_sharedToken))
                {
                    _cachedToken = _sharedToken;
                    Console.WriteLine($"[TokenStorage] Returning shared token. Length: {_sharedToken.Length}");
                    _logger.LogInformation("Token retrieved from shared cache");
                    return _cachedToken;
                }
                else
                {
                    Console.WriteLine($"[TokenStorage] Shared token is NULL or EMPTY");
                }
            }

            // 2. Пробуем загрузить из cookies (доступно на сервере, работает при prerendering)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                Console.WriteLine($"[TokenStorage] HttpContext is available. Checking cookies...");
                if (httpContext.Request.Cookies.TryGetValue(TokenKey, out var cookieToken))
                {
                    if (!string.IsNullOrEmpty(cookieToken))
                    {
                        _cachedToken = cookieToken.Trim().Trim('"');
                        
                        // Сохраняем в общий кэш для использования другими экземплярами (HttpClient)
                        lock (_lock)
                        {
                            _sharedToken = _cachedToken;
                            Console.WriteLine($"[TokenStorage] Token saved to shared cache from cookie. Length: {_sharedToken.Length}");
                        }
                        
                        Console.WriteLine($"[TokenStorage] Token retrieved from cookie and cached: {_cachedToken.Substring(0, Math.Min(10, _cachedToken.Length))}...");
                        _logger.LogInformation("Token successfully retrieved from cookie");
                        return _cachedToken;
                    }
                    else
                    {
                        Console.WriteLine($"[TokenStorage] Cookie '{TokenKey}' exists but is empty");
                    }
                }
                else
                {
                    Console.WriteLine($"[TokenStorage] Cookie '{TokenKey}' not found in request. Available cookies: {string.Join(", ", httpContext.Request.Cookies.Keys)}");
                }
            }
            else
            {
                Console.WriteLine($"[TokenStorage] HttpContext is NULL - cannot read cookies");
            }

            // 3. Пробуем загрузить из localStorage (для клиентской стороны)
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                
                Console.WriteLine($"[TokenStorage] Token from localStorage: {(string.IsNullOrEmpty(token) ? "NULL or EMPTY" : "EXISTS")}");
                
                if (!string.IsNullOrEmpty(token))
                {
                    // Очищаем от лишних кавычек и пробелов
                    _cachedToken = token.Trim().Trim('"');
                    
                    // Сохраняем в общий кэш для использования другими экземплярами (HttpClient)
                    lock (_lock)
                    {
                        _sharedToken = _cachedToken;
                        Console.WriteLine($"[TokenStorage] Token saved to shared cache. Length: {_sharedToken.Length}");
                    }
                    
                    Console.WriteLine($"[TokenStorage] Token retrieved from localStorage and cached: {_cachedToken.Substring(0, Math.Min(10, _cachedToken.Length))}...");
                    _logger.LogInformation("Token successfully retrieved from localStorage");
                    return _cachedToken;
                }
            }
            catch (Exception ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                // Это нормальная ситуация при пре-рендеринге - уже проверили cookies выше
                Console.WriteLine($"[TokenStorage] JS Interop not available (Prerendering), but cookies were checked first");
            }
            catch (Exception ex)
            {
                // Другие ошибки JS
                Console.WriteLine($"[TokenStorage] ERROR retrieving token from localStorage: {ex.Message}");
                _logger.LogWarning($"Error retrieving token from localStorage: {ex.Message}");
            }

            Console.WriteLine($"[TokenStorage] Returning NULL - no token found");
            return null;
        }

        public async Task SetTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[TokenStorage] SetTokenAsync called with NULL or EMPTY token!");
                _logger.LogWarning("SetTokenAsync called with empty token");
                return;
            }

            Console.WriteLine($"[TokenStorage] SetTokenAsync called with token length: {token.Length}");
            Console.WriteLine($"[TokenStorage] Token preview: {token.Substring(0, Math.Min(20, token.Length))}...");
            
            _cachedToken = token;
            
            // Сохраняем в общий кэш (для HttpClient запросов)
            lock (_lock)
            {
                _sharedToken = token;
                Console.WriteLine($"[TokenStorage] Token saved to shared cache. Shared token length: {(_sharedToken?.Length ?? 0)}");
                Console.WriteLine($"[TokenStorage] Shared token preview: {(_sharedToken != null ? _sharedToken.Substring(0, Math.Min(20, _sharedToken.Length)) : "NULL")}...");
                _logger.LogInformation("Token saved to shared cache");
            }
            
            // Сохраняем в cookies (доступно на сервере, работает при prerendering)
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                try
                {
                    // Проверяем, можно ли еще записывать в ответ
                    if (!httpContext.Response.HasStarted)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = false, // Нужно false, чтобы JS мог читать
                            Secure = false, // В production должно быть true для HTTPS
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddDays(7) // Токен живет 7 дней
                        };
                        
                        httpContext.Response.Cookies.Append(TokenKey, token, cookieOptions);
                        Console.WriteLine("[TokenStorage] Token saved to cookie successfully");
                        _logger.LogInformation("Token successfully saved to cookie");
                    }
                    else
                    {
                        Console.WriteLine("[TokenStorage] WARNING: Response has already started - cannot save token to cookie (but saved to shared cache and localStorage)!");
                        _logger.LogWarning("Response has already started - token not saved to cookie, but saved to shared cache and localStorage");
                    }
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("read-only") || ex.Message.Contains("already started"))
                {
                    // Ответ уже отправлен - это нормально при логине через HttpClient
                    Console.WriteLine("[TokenStorage] WARNING: Cannot save to cookie - response already started (but saved to shared cache and localStorage)!");
                    _logger.LogWarning("Cannot save to cookie - response already started, but token saved to shared cache and localStorage");
                }
            }
            else
            {
                Console.WriteLine("[TokenStorage] WARNING: HttpContext is NULL - cannot save token to cookie (but saved to shared cache)!");
                _logger.LogWarning("HttpContext is NULL - token not saved to cookie, but saved to shared cache");
            }
            
            // Также сохраняем в localStorage (для клиентской стороны)
            try
            {
                Console.WriteLine($"[TokenStorage] Attempting to save token to localStorage with key: {TokenKey}");
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
                Console.WriteLine("[TokenStorage] Token saved to localStorage successfully");
                _logger.LogInformation("Token successfully saved to localStorage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenStorage] ERROR saving token to localStorage: {ex.Message}");
                _logger.LogWarning(ex, "Error saving token to localStorage (but cookie was saved)");
            }
        }

        public async Task RemoveTokenAsync()
        {
            // Логируем стек вызовов ПЕРЕД удалением, чтобы понять, откуда вызывается
            var stackTrace = Environment.StackTrace;
            Console.WriteLine($"[TokenStorage] RemoveTokenAsync called from:");
            Console.WriteLine($"[TokenStorage] StackTrace: {stackTrace}");
            _logger.LogWarning("Token is being removed. Stack trace: {StackTrace}", stackTrace);
            
            _cachedToken = null;
            
            // Удаляем из общего кэша
            lock (_lock)
            {
                _sharedToken = null;
                Console.WriteLine("[TokenStorage] Token removed from shared cache");
            }
            
            // Удаляем из cookies
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Delete(TokenKey);
                Console.WriteLine("[TokenStorage] Token removed from cookie");
                _logger.LogInformation("Token successfully removed from cookie");
            }
            
            // Удаляем из localStorage
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                Console.WriteLine("[TokenStorage] Token removed from localStorage");
                _logger.LogInformation("Token successfully removed from localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error removing token from localStorage: {ex.Message}");
            }
        }

        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
