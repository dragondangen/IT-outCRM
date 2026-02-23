using Microsoft.JSInterop;
using Microsoft.AspNetCore.Http;

namespace IT_outCRM.Blazor.Services
{
    /// <summary>
    /// Per-circuit token storage. Each Blazor Server circuit (user session)
    /// gets its own Scoped instance — tokens are never shared between users.
    /// Cookie is HttpOnly+Secure (read only server-side), localStorage is the client-side fallback.
    /// </summary>
    public class TokenStorage : ITokenStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenStorage> _logger;
        private const string TokenKey = "authToken";
        private string? _cachedToken;

        private static readonly CookieOptions SecureCookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };

        public TokenStorage(
            IJSRuntime jsRuntime,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TokenStorage> logger)
        {
            _jsRuntime = jsRuntime;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken))
                return _cachedToken;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (httpContext.Request.Cookies.TryGetValue(TokenKey, out var cookieToken)
                    && !string.IsNullOrEmpty(cookieToken))
                {
                    _cachedToken = cookieToken.Trim().Trim('"');
                    return _cachedToken;
                }
            }

            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                if (!string.IsNullOrEmpty(token))
                {
                    _cachedToken = token.Trim().Trim('"');
                    return _cachedToken;
                }
            }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read token from localStorage");
            }

            return null;
        }

        public async Task SetTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return;

            _cachedToken = token;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                try
                {
                    if (!httpContext.Response.HasStarted)
                    {
                        httpContext.Response.Cookies.Append(TokenKey, token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Path = "/",
                            Expires = DateTimeOffset.UtcNow.AddDays(7)
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set auth cookie");
                }
            }

            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
            }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set token in localStorage");
            }
        }

        public async Task RemoveTokenAsync()
        {
            _cachedToken = null;

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null && !httpContext.Response.HasStarted)
                {
                    httpContext.Response.Cookies.Delete(TokenKey, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Path = "/"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete auth cookie");
            }

            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
            }
            catch (InvalidOperationException) { }
            catch (JSDisconnectedException) { }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove token from localStorage");
            }
        }

        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
