using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;

namespace IT_outCRM.Blazor.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;

        public CompanyService(HttpClient httpClient)
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

        public async Task<List<CompanyModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/companies");
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[CompanyService] GetAll failed: {response.StatusCode} - {error}");
                    // Для отладки можно вернуть пустой список, но лучше бы знать, что ошибка
                    throw new Exception($"Ошибка загрузки ({response.StatusCode}): {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<CompanyModel>>();
                Console.WriteLine($"[CompanyService] Loaded {result?.Count ?? 0} companies");
                return result ?? new List<CompanyModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompanyService] Exception loading companies: {ex.Message}");
                throw; // Пробрасываем ошибку, чтобы UI её показал
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
                Console.WriteLine($"[CompanyService] Error getting company {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<CompanyModel?> CreateAsync(CompanyModel model)
        {
            try
            {
                Console.WriteLine($"[CompanyService] Creating company: {model.Name}");
                var response = await _httpClient.PostAsJsonAsync("api/companies", model);
                
                Console.WriteLine($"[CompanyService] Create Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CompanyModel>();
                }
                else 
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[CompanyService] Create Error Body: {error}");
                    throw new Exception($"Ошибка сервера ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompanyService] Error creating company: {ex.Message}");
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
                Console.WriteLine($"[CompanyService] Error updating company: {ex.Message}");
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
                     Console.WriteLine($"[CompanyService] Delete failed: {error}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompanyService] Error deleting company: {ex.Message}");
                return false;
            }
        }
    }
}
