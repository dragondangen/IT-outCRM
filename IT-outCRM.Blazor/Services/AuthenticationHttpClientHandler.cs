using System.Net.Http.Headers;

namespace IT_outCRM.Blazor.Services
{
    public class AuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthenticationHttpClientHandler> _logger;

        public AuthenticationHttpClientHandler(
            CustomAuthenticationStateProvider authStateProvider, 
            ILogger<AuthenticationHttpClientHandler> logger)
        {
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Получаем токен через AuthStateProvider (он должен быть Scoped и иметь кэш)
                var token = await _authStateProvider.GetTokenAsync();
                
                // Если токен не получен, пробуем еще раз после небольшой задержки
                // Это помогает при первом запросе, когда токен еще не загружен
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"[AuthHandler] Token not found on first attempt, retrying after delay...");
                    await Task.Delay(100);
                    token = await _authStateProvider.GetTokenAsync();
                }
                
                if (!string.IsNullOrEmpty(token))
                {
                    // CLEANUP: Ensure no extra quotes or spaces
                    token = token.Trim().Trim('"');
                    
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine($"[AuthHandler] Attached token to {request.RequestUri}");
                    Console.WriteLine($"[AuthHandler] Token length: {token.Length}, preview: {token.Substring(0, Math.Min(20, token.Length))}...");
                }
                else
                {
                    Console.WriteLine($"[AuthHandler] WARNING: No token available for {request.RequestUri}");
                    _logger.LogWarning($"No token found for request {request.RequestUri}. Check if you are logged in.");
                }
            }
            catch (Exception ex)
            {
                // Log specifically if it's a JS Interop error (Prerendering issue)
                if (ex.Message.Contains("JavaScript"))
                {
                     _logger.LogWarning($"Skipping token attachment due to Prerendering: {ex.Message}");
                }
                else
                {
                    _logger.LogError(ex, "Error attaching token");
                    Console.WriteLine($"[AuthHandler] Exception: {ex.Message}");
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning($"Request to {request.RequestUri} returned 401 Unauthorized");
                Console.WriteLine($"[AuthHandler] 401 Unauthorized from {request.RequestUri}");
                
                // Try to log WWW-Authenticate header if present
                if (response.Headers.WwwAuthenticate.Any())
                {
                    foreach (var header in response.Headers.WwwAuthenticate)
                    {
                        Console.WriteLine($"[AuthHandler] WWW-Authenticate: {header}");
                    }
                }
                
                // НЕ удаляем токен автоматически при 401 - это может быть временная ошибка
                // Токен должен удаляться только при явном logout
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogWarning($"Request to {request.RequestUri} returned 403 Forbidden");
                Console.WriteLine($"[AuthHandler] 403 Forbidden from {request.RequestUri}");
                Console.WriteLine($"[AuthHandler] Token is still valid, but user doesn't have required permissions");
                
                // НЕ удаляем токен при 403 - это ошибка прав доступа, а не авторизации
                // Токен валиден, но у пользователя нет прав на этот ресурс
            }

            return response;
        }
    }
}
