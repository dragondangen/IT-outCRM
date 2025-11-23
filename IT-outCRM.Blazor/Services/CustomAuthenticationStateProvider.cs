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

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            
            _logger.LogInformation("GetAuthenticationStateAsync called. Token present: {TokenPresent}", !string.IsNullOrEmpty(token));

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            
            _logger.LogInformation("User authenticated: {Username}", user.Identity?.Name ?? "Unknown");

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
                return token.Claims;
            }
            catch
            {
                return new List<Claim>();
            }
        }
    }
}



