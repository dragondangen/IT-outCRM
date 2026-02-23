using IT_outCRM.Blazor.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContactPersonService> _logger;

        public ContactPersonService(HttpClient httpClient, ILogger<ContactPersonService> logger)
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

        public async Task<List<ContactPersonModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/contactpersons");
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GetAll failed: {StatusCode} - {Error}", response.StatusCode, error);
                    throw new Exception($"Ошибка загрузки ({response.StatusCode}): {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<List<ContactPersonModel>>();
                return result ?? new List<ContactPersonModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception loading contact persons");
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
                _logger.LogWarning(ex, "Error getting contact person {Id}", id);
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
                _logger.LogWarning(ex, "Error creating contact person");
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
                _logger.LogWarning(ex, "Error updating contact person");
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
                _logger.LogWarning(ex, "Error deleting contact person");
                return false;
            }
        }
    }
}













