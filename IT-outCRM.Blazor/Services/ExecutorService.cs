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
                // Конвертируем модель в DTO для бэкенда
                var createDto = new
                {
                    AccountId = model.AccountId,
                    CompanyId = model.CompanyId,
                    CompletedOrders = model.CompletedOrders
                };
                
                var response = await _httpClient.PostAsJsonAsync("api/executors", createDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ExecutorModel>();
                }
                else 
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ExecutorService] Create failed: {response.StatusCode} - {errorContent}");
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
                // Конвертируем модель в DTO для бэкенда
                var updateDto = new
                {
                    Id = model.Id,
                    AccountId = model.AccountId,
                    CompanyId = model.CompanyId,
                    CompletedOrders = model.CompletedOrders
                };
                
                var response = await _httpClient.PutAsJsonAsync($"api/executors/{model.Id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                     if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<ExecutorModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ExecutorService] Update failed: {response.StatusCode} - {errorContent}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExecutorService] Error updating executor: {ex.Message}");
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
