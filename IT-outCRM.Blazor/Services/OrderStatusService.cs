using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly HttpClient _httpClient;

        public OrderStatusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderStatusModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderStatusModel>>("api/orderstatuses");
                return result ?? new List<OrderStatusModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order statuses: {ex.Message}");
                return new List<OrderStatusModel>();
            }
        }
    }
}

