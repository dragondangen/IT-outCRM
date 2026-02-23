using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountService> _logger;

        public AccountService(HttpClient httpClient, ILogger<AccountService> logger)
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

        public async Task<List<AccountModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<AccountModel>>("api/accounts");
                _logger.LogDebug("Loaded {Count} accounts", result?.Count ?? 0);
                return result ?? new List<AccountModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error loading accounts");
                return new List<AccountModel>();
            }
        }

        public async Task<AccountModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<AccountModel>($"api/accounts/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting account {Id}", id);
                return null;
            }
        }

        public async Task<AccountModel?> CreateAsync(AccountModel model)
        {
            try
            {
                _logger.LogDebug("Creating account for company {CompanyName}", model.CompanyName);
                var response = await _httpClient.PostAsJsonAsync("api/accounts", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AccountModel>();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Create failed: {StatusCode} - {Error}", response.StatusCode, err);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception creating account");
                throw;
            }
        }

        public async Task<AccountModel?> UpdateAsync(AccountModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/accounts/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                     if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<AccountModel>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error updating account");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/accounts/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting account");
                return false;
            }
        }
    }
}
