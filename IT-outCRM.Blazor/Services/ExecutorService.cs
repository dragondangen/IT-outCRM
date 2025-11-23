using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class ExecutorService : IExecutorService
    {
        private readonly HttpClient _httpClient;

        public ExecutorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ExecutorModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ExecutorModel>>("api/executors");
                return result ?? new List<ExecutorModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading executors: {ex.Message}");
                return new List<ExecutorModel>();
            }
        }

        public async Task<ExecutorModel?> CreateAsync(ExecutorModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/executors", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ExecutorModel>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating executor: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/executors/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting executor: {ex.Message}");
                return false;
            }
        }
    }
}



