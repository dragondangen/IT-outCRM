using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;

namespace IT_outCRM.Blazor.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly HttpClient _httpClient;

        public ContactPersonService(HttpClient httpClient)
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

        public async Task<List<ContactPersonModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/contactpersons");
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ContactPersonService] GetAll failed: {response.StatusCode} - {error}");
                    throw new Exception($"Ошибка загрузки ({response.StatusCode}): {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<ContactPersonModel>>();
                return result ?? new List<ContactPersonModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactPersonService] Exception loading contact persons: {ex.Message}");
                throw;
            }
        }

        public async Task<ContactPersonModel?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ContactPersonModel>($"api/contactpersons/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactPersonService] Error getting contact person {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<ContactPersonModel?> CreateAsync(ContactPersonModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/contactpersons", model);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContactPersonModel>();
                }
                else 
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка сервера ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactPersonService] Error creating contact person: {ex.Message}");
                throw;
            }
        }

        public async Task<ContactPersonModel?> UpdateAsync(ContactPersonModel model)
        {
             try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/contactpersons/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return model;
                    return await response.Content.ReadFromJsonAsync<ContactPersonModel>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка сервера ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactPersonService] Error updating contact person: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/contactpersons/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactPersonService] Error deleting contact person: {ex.Message}");
                return false;
            }
        }
    }
}




