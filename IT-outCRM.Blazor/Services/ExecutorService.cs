using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;

namespace IT_outCRM.Blazor.Services
{
    public class ExecutorService : IExecutorService
    {
        private readonly HttpClient _httpClient;

        public ExecutorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ExecutorModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ExecutorModel>>("api/executors");
                Console.WriteLine($"[ExecutorService] Loaded {result?.Count ?? 0} executors");
                return result ?? new List<ExecutorModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExecutorService] Error loading executors: {ex.Message}");
                return new List<ExecutorModel>();
            }
        }

        public async Task<ExecutorModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ExecutorModel>($"api/executors/{id}");
            }
            catch
            {
                return null;
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
                else 
                {
                    Console.WriteLine($"[ExecutorService] Create failed: {response.StatusCode}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExecutorService] Error creating executor: {ex.Message}");
                throw;
            }
        }

        public async Task<ExecutorModel?> UpdateAsync(ExecutorModel model)
        {
             try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/executors/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                     if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<ExecutorModel>();
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
                var response = await _httpClient.DeleteAsync($"api/executors/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExecutorService] Error deleting executor: {ex.Message}");
                return false;
            }
        }
    }
}
