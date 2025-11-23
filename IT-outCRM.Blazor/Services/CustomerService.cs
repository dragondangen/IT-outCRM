using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CustomerModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<CustomerModel>>("api/customers");
                return result ?? new List<CustomerModel>();
            }
            catch
            {
                return new List<CustomerModel>();
            }
        }

        public async Task<PagedResult<CustomerModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PagedResult<CustomerModel>>(
                    $"api/customers/paged?pageNumber={pageNumber}&pageSize={pageSize}");
                return result ?? new PagedResult<CustomerModel>();
            }
            catch
            {
                return new PagedResult<CustomerModel>();
            }
        }

        public async Task<CustomerModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CustomerModel>($"api/customers/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<CustomerModel?> CreateAsync(CreateCustomerModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/customers", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerModel>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CustomerModel?> UpdateAsync(CustomerModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/customers/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerModel>();
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
                var response = await _httpClient.DeleteAsync($"api/customers/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

