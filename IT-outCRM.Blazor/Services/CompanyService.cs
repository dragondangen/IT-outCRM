using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(HttpClient httpClient, ILogger<CompanyService> logger)
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

        public async Task<List<CompanyModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/companies");
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GetAll failed: {StatusCode} - {Error}", response.StatusCode, error);
                    throw new Exception($"Ошибка загрузки ({response.StatusCode}): {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<CompanyModel>>();
                _logger.LogDebug("Loaded {Count} companies", result?.Count ?? 0);
                return result ?? new List<CompanyModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception loading companies");
                throw;
            }
        }

        public async Task<CompanyModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CompanyModel>($"api/companies/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting company {Id}", id);
                return null;
            }
        }

        public async Task<CompanyModel?> CreateAsync(CompanyModel model)
        {
            try
            {
                _logger.LogDebug("Creating company: {Name}", model.Name);
                var response = await _httpClient.PostAsJsonAsync("api/companies", model);
                _logger.LogDebug("Create Status: {StatusCode}", response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CompanyModel>();
                }
                else 
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Create Error Body: {Error}", error);
                    throw new Exception($"Ошибка сервера ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating company");
                throw;
            }
        }

        public async Task<CompanyModel?> UpdateAsync(CompanyModel model)
        {
             try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/companies/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<CompanyModel>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка сервера ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error updating company");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/companies/{id}");
                if (!response.IsSuccessStatusCode)
                {
                     var error = await response.Content.ReadAsStringAsync();
                     _logger.LogWarning("Delete failed: {Error}", error);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting company");
                return false;
            }
        }
    }
}
