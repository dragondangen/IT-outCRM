using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IT_outCRM.Blazor.Services
{
    public class BlazorAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<BlazorAuthenticationHandler> _logger;

        public BlazorAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder)
            : base(options, loggerFactory, encoder)
        {
            _logger = loggerFactory.CreateLogger<BlazorAuthenticationHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Аутентификация обрабатывается через CustomAuthenticationStateProvider
            // Этот handler просто возвращает успех для всех запросов
            // Blazor Server будет использовать CustomAuthenticationStateProvider для проверки авторизации
            var claims = new[] { new Claim(ClaimTypes.Name, "Anonymous") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            // Не выполняем редирект при 403 - это обрабатывается в Routes.razor
            // Просто возвращаем успех, чтобы Blazor Server мог обработать это через CustomAuthenticationStateProvider
            // Не логируем это как ошибку, так как это нормальное поведение для Blazor Server
            _logger.LogDebug("Forbidden request handled by BlazorAuthenticationHandler - authorization will be checked by CustomAuthenticationStateProvider");
            return Task.CompletedTask;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Не выполняем редирект - это обрабатывается в Routes.razor
            // Не логируем это как ошибку, так как это нормальное поведение для Blazor Server
            _logger.LogDebug("Challenge request handled by BlazorAuthenticationHandler - authentication will be checked by CustomAuthenticationStateProvider");
            return Task.CompletedTask;
        }
    }
}


