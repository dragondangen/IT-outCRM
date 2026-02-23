using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class ExecutorService : IExecutorService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExecutorService> _logger;

        public ExecutorService(HttpClient httpClient, ILogger<ExecutorService> logger)
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

        public async Task<List<ExecutorModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ExecutorModel>>("api/executors");
                _logger.LogDebug("Loaded {Count} executors", result?.Count ?? 0);
                return result ?? new List<ExecutorModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading executors");
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
                    _logger.LogWarning("Create failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating executor");
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
                    _logger.LogWarning("Update failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error updating executor");
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
                _logger.LogWarning(ex, "Error deleting executor");
                return false;
            }
        }
    }
}
