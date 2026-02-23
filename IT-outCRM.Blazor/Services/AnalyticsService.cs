using System.Net.Http.Json;
using System.Text.Json;
using IT_outCRM.Blazor.Models;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AnalyticsService> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AnalyticsService(HttpClient httpClient, ILogger<AnalyticsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DashboardAnalyticsModel?> GetDashboardAnalyticsAsync()
        {
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/analytics/dashboard");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _logger.LogWarning("Analytics: 401 on attempt {Attempt}, retrying...", attempt);
                        await Task.Delay(attempt * 300);
                        continue;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Analytics: HTTP {StatusCode} on attempt {Attempt}", (int)response.StatusCode, attempt);
                        return null;
                    }

                    return await response.Content.ReadFromJsonAsync<DashboardAnalyticsModel>(JsonOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Analytics: exception on attempt {Attempt}", attempt);
                    if (attempt < 3)
                        await Task.Delay(attempt * 300);
                }
            }
            return null;
        }
    }
}
