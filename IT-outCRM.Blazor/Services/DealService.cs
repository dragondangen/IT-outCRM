using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class DealService : IDealService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<DealModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/deals");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DealModel>>(JsonOptions) ?? new();
        }

        public async Task<DealModel?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/deals/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DealModel>(JsonOptions);
        }

        public async Task<List<DealModel>> GetMyDealsAsync()
        {
            var response = await _httpClient.GetAsync("api/deals/my-deals");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DealModel>>(JsonOptions) ?? new();
        }

        public async Task<DealModel?> CreateFromOrderAsync(CreateDealFromOrderModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/deals/from-order", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            return await response.Content.ReadFromJsonAsync<DealModel>(JsonOptions);
        }

        public async Task<DealModel?> ChangeStatusAsync(Guid dealId, string newStatus)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/deals/{dealId}/status", new { status = newStatus });
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            return await response.Content.ReadFromJsonAsync<DealModel>(JsonOptions);
        }

        public async Task<List<DealMessageModel>> GetMessagesAsync(Guid dealId)
        {
            var response = await _httpClient.GetAsync($"api/deals/{dealId}/messages");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DealMessageModel>>(JsonOptions) ?? new();
        }

        public async Task<DealMessageModel?> AddMessageAsync(Guid dealId, string text)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/deals/{dealId}/messages", new { text });
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            return await response.Content.ReadFromJsonAsync<DealMessageModel>(JsonOptions);
        }

        public async Task<DealModel?> RateByCustomerAsync(Guid dealId, int rating, string? review)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/deals/{dealId}/rate-customer", new { rating, review });
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            return await response.Content.ReadFromJsonAsync<DealModel>(JsonOptions);
        }

        public async Task<DealModel?> RateByExecutorAsync(Guid dealId, int rating, string? review)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/deals/{dealId}/rate-executor", new { rating, review });
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            return await response.Content.ReadFromJsonAsync<DealModel>(JsonOptions);
        }

        public async Task<List<DealModel>> GetByOrderAsync(Guid orderId)
        {
            var response = await _httpClient.GetAsync($"api/deals/by-order/{orderId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<DealModel>>(JsonOptions) ?? new();
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/deals/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
