using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderStatusService> _logger;

        public OrderStatusService(HttpClient httpClient, ILogger<OrderStatusService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<OrderStatusModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderStatusModel>>("api/orderstatuses");
                _logger.LogDebug("Loaded {Count} statuses", result?.Count ?? 0);
                return result ?? new List<OrderStatusModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading order statuses");
                return new List<OrderStatusModel>();
            }
        }

        public async Task<OrderStatusModel?> GetByIdAsync(Guid id)
        {
             try
            {
                return await _httpClient.GetFromJsonAsync<OrderStatusModel>($"api/orderstatuses/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<OrderStatusModel?> CreateAsync(OrderStatusModel model)
        {
             try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orderstatuses", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderStatusModel>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating status");
                throw;
            }
        }

        public async Task<OrderStatusModel?> UpdateAsync(OrderStatusModel model)
        {
             try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orderstatuses/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<OrderStatusModel>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/orderstatuses/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
