using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace IT_outCRM.Blazor.Services
{
    public class TokenStorage : ITokenStorage
    {
        private readonly ProtectedLocalStorage _localStorage;
        private const string TokenKey = "authToken";
        private string? _cachedToken; // In-memory cache для быстрого доступа
        private readonly ILogger<TokenStorage> _logger;

        public TokenStorage(
            ProtectedLocalStorage localStorage,
            ILogger<TokenStorage> logger)
        {
            _localStorage = localStorage;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            // Сначала проверяем кэш
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                _logger.LogInformation("Token retrieved from cache");
                return _cachedToken;
            }

            // Затем пробуем загрузить из localStorage
            try
            {
                var result = await _localStorage.GetAsync<string>(TokenKey);
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    _cachedToken = result.Value;
                    _logger.LogInformation("Token retrieved from localStorage");
                    return _cachedToken;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving token from localStorage");
            }

            _logger.LogInformation("No token found");
            return null;
        }

        public async Task SetTokenAsync(string token)
        {
            _cachedToken = token;
            _logger.LogInformation("Token cached in memory");
            
            try
            {
                await _localStorage.SetAsync(TokenKey, token);
                _logger.LogInformation("Token saved to localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error saving token to localStorage");
            }
        }

        public async Task RemoveTokenAsync()
        {
            _cachedToken = null;
            _logger.LogInformation("Token removed from cache");
            
            try
            {
                await _localStorage.DeleteAsync(TokenKey);
                _logger.LogInformation("Token removed from localStorage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing token from localStorage");
            }
        }

        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}



