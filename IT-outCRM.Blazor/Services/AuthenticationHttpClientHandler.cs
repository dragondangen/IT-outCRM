using System.Net.Http.Headers;

namespace IT_outCRM.Blazor.Services
{
    public class AuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public AuthenticationHttpClientHandler(
            CustomAuthenticationStateProvider authStateProvider,
            ILogger<AuthenticationHttpClientHandler> logger)
        {
            _authStateProvider = authStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var token = await _authStateProvider.GetTokenAsync();

                if (string.IsNullOrEmpty(token))
                {
                    for (int i = 0; i < 5 && string.IsNullOrEmpty(token); i++)
                    {
                        await Task.Delay(200);
                        token = await _authStateProvider.GetTokenAsync();
                    }
                }

                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Trim().Trim('"');
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch { /* JS interop or other errors during prerender */ }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
