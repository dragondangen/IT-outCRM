using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;

namespace IT_outCRM.Blazor.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
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

        public async Task<List<AccountModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<AccountModel>>("api/accounts");
                Console.WriteLine($"[AccountService] Loaded {result?.Count ?? 0} accounts");
                return result ?? new List<AccountModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountService] Error loading accounts: {ex.Message}");
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
                Console.WriteLine($"[AccountService] Error getting account {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<AccountModel?> CreateAsync(AccountModel model)
        {
            try
            {
                // Убедимся, что отправляем правильные данные
                Console.WriteLine($"[AccountService] Creating account for company {model.CompanyName}...");
                
                var response = await _httpClient.PostAsJsonAsync("api/accounts", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AccountModel>();
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AccountService] Create failed: {response.StatusCode} - {err}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountService] Exception creating account: {ex.Message}");
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
                Console.WriteLine($"[AccountService] Error updating account: {ex.Message}");
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
                Console.WriteLine($"[AccountService] Error deleting account: {ex.Message}");
                return false;
            }
        }
    }
}
