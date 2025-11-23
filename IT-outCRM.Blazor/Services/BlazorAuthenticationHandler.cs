using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IT_outCRM.Blazor.Services
{
    public class BlazorAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BlazorAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Аутентификация обрабатывается через CustomAuthenticationStateProvider
            // Этот handler просто возвращает успех для всех запросов
            var claims = new[] { new Claim(ClaimTypes.Name, "Anonymous") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Не выполняем редирект - это обрабатывается в Routes.razor
            return Task.CompletedTask;
        }
    }
}

