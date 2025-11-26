using Microsoft.JSInterop;

namespace IT_outCRM.Blazor.Services
{
    public class TokenStorage : ITokenStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "authToken";
        private string? _cachedToken; // In-memory cache
        private readonly ILogger<TokenStorage> _logger;

        public TokenStorage(
            IJSRuntime jsRuntime,
            ILogger<TokenStorage> logger)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
            Console.WriteLine($"[TokenStorage] Created instance {this.GetHashCode()}");
        }

        public async Task<string?> GetTokenAsync()
        {
            Console.WriteLine($"[TokenStorage {this.GetHashCode()}] GetTokenAsync called. Cached: {!string.IsNullOrEmpty(_cachedToken)}");
            
            // 1. Проверяем кэш
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                return _cachedToken;
            }

            // 2. Пробуем загрузить из localStorage
            try
            {
                // Используем прямой вызов localStorage, так как это стандарт API браузера
                // Пытаемся вызвать JS. Если это пре-рендеринг, здесь вылетит исключение.
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                
                if (!string.IsNullOrEmpty(token))
                {
                    // Очищаем от лишних кавычек и пробелов
                    _cachedToken = token.Trim().Trim('"');
                    Console.WriteLine($"[TokenStorage] Token retrieved: {_cachedToken.Substring(0, Math.Min(10, _cachedToken.Length))}...");
                    return _cachedToken;
                }
            }
            catch (Exception ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                // Это нормальная ситуация при пре-рендеринге. Просто молча возвращаем null.
                // _logger.LogDebug("JS Interop not available yet (Prerendering)");
                return null;
            }
            catch (Exception ex)
            {
                // Другие ошибки JS
                _logger.LogWarning($"Error retrieving token: {ex.Message}");
                return null;
            }

            return null;
        }

        public async Task SetTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return;

            _cachedToken = token;
            
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
                Console.WriteLine("[TokenStorage] Token saved to localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error saving token: {ex.Message}");
            }
        }

        public async Task RemoveTokenAsync()
        {
            _cachedToken = null;
            
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                Console.WriteLine("[TokenStorage] Token removed from localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error removing token: {ex.Message}");
            }
        }

        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
