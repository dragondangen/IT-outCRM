using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AnalyticsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardAnalyticsModel?> GetDashboardAnalyticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/analytics/dashboard");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<DashboardAnalyticsModel>(JsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
