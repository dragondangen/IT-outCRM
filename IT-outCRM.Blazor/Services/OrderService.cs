using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderModel>>("api/orders");
                return result ?? new List<OrderModel>();
            }
            catch
            {
                return new List<OrderModel>();
            }
        }

        public async Task<PagedResult<OrderModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PagedResult<OrderModel>>(
                    $"api/orders/paged?pageNumber={pageNumber}&pageSize={pageSize}");
                return result ?? new PagedResult<OrderModel>();
            }
            catch
            {
                return new PagedResult<OrderModel>();
            }
        }

        public async Task<OrderModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderModel>($"api/orders/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<OrderModel?> CreateAsync(CreateOrderModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderModel>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<OrderModel?> UpdateAsync(OrderModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orders/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderModel>();
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
                var response = await _httpClient.DeleteAsync($"api/orders/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<OrderModel>> GetByCustomerAsync(Guid customerId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderModel>>(
                    $"api/orders/by-customer/{customerId}");
                return result ?? new List<OrderModel>();
            }
            catch
            {
                return new List<OrderModel>();
            }
        }
    }
}

