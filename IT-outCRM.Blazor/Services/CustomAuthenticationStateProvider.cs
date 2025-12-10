using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace IT_outCRM.Blazor.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;

        public CustomAuthenticationStateProvider(
            ITokenStorage tokenStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _tokenStorage = tokenStorage;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _tokenStorage.GetTokenAsync();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            
            _logger.LogInformation("GetAuthenticationStateAsync called. Token present: {TokenPresent}", !string.IsNullOrEmpty(token));
            Console.WriteLine($"[AuthProvider] GetAuthenticationStateAsync called. Token present: {!string.IsNullOrEmpty(token)}");

            // Если токен не найден, пробуем еще раз после небольшой задержки
            // Это помогает при prerendering, когда JS Interop может быть недоступен
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[AuthProvider] No token found on first attempt, retrying after delay...");
                await Task.Delay(100);
                token = await _tokenStorage.GetTokenAsync();
                Console.WriteLine($"[AuthProvider] Retry result. Token present: {!string.IsNullOrEmpty(token)}");
            }

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[AuthProvider] No token found after retry, returning unauthenticated state");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            var user = new ClaimsPrincipal(identity);
            
            _logger.LogInformation("User authenticated: {Username}", user.Identity?.Name ?? "Unknown");
            Console.WriteLine($"[AuthProvider] User authenticated: {user.Identity?.Name ?? "Unknown"}");
            Console.WriteLine($"[AuthProvider] IsAuthenticated: {user.Identity?.IsAuthenticated}");
            
            // Проверяем роли
            var roles = claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToList();
            _logger.LogInformation("User roles: {Roles}", string.Join(", ", roles));
            Console.WriteLine($"[AuthProvider] User roles: {string.Join(", ", roles)}");
            Console.WriteLine($"[AuthProvider] IsInRole('Executor'): {user.IsInRole("Executor")}");

            return new AuthenticationState(user);
        }

        public void NotifyAuthenticationStateChanged()
        {
            _logger.LogInformation("NotifyAuthenticationStateChanged called");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var claims = token.Claims.ToList();
                
                // Логируем все claims для отладки
                _logger.LogInformation("Parsing JWT token. Claims count: {Count}", claims.Count);
                foreach (var claim in claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                    Console.WriteLine($"[AuthProvider] Claim: {claim.Type} = {claim.Value}");
                }
                
                // Проверяем, есть ли роли в разных форматах
                var roleClaims = claims.Where(c => 
                    c.Type == ClaimTypes.Role || 
                    c.Type == "role" || 
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                    c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase)).ToList();
                
                _logger.LogInformation("Role claims found: {Count}", roleClaims.Count);
                foreach (var roleClaim in roleClaims)
                {
                    _logger.LogInformation("Role claim: {Type} = {Value}", roleClaim.Type, roleClaim.Value);
                    Console.WriteLine($"[AuthProvider] Role claim: {roleClaim.Type} = {roleClaim.Value}");
                }
                
                // Если роли есть, но не в стандартном формате, добавляем их как ClaimTypes.Role
                var resultClaims = new List<Claim>(claims);
                foreach (var roleClaim in roleClaims)
                {
                    if (roleClaim.Type != ClaimTypes.Role)
                    {
                        // Добавляем роль в стандартном формате для IsInRole()
                        resultClaims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                        _logger.LogInformation("Added role claim as ClaimTypes.Role: {Role}", roleClaim.Value);
                    }
                }
                
                return resultClaims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT token");
                Console.WriteLine($"[AuthProvider] ERROR parsing JWT: {ex.Message}");
                return new List<Claim>();
            }
        }
    }
}



