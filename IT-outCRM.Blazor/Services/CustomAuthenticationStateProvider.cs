using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace IT_outCRM.Blazor.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStorage _tokenStorage;

        public CustomAuthenticationStateProvider(
            ITokenStorage tokenStorage,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _tokenStorage = tokenStorage;
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _tokenStorage.GetTokenAsync();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                await Task.Delay(100);
                token = await _tokenStorage.GetTokenAsync();
            }

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var claims = token.Claims.ToList();

                var roleClaims = claims.Where(c =>
                    c.Type == ClaimTypes.Role ||
                    c.Type == "role" ||
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                    c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase)).ToList();

                var resultClaims = new List<Claim>(claims);
                foreach (var roleClaim in roleClaims)
                {
                    if (roleClaim.Type != ClaimTypes.Role)
                        resultClaims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                return resultClaims;
            }
            catch
            {
                return new List<Claim>();
            }
        }
    }
}
