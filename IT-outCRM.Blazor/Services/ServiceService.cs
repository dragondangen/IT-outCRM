using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class ServiceService : IServiceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(HttpClient httpClient, ILogger<ServiceService> logger)
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

        public async Task<List<ServiceModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ServiceModel>>("api/services");
                return result ?? new List<ServiceModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetAllAsync error");
                return new List<ServiceModel>();
            }
        }

        public async Task<PagedResult<ServiceModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PagedResult<ServiceModel>>(
                    $"api/services/paged?pageNumber={pageNumber}&pageSize={pageSize}");
                return result ?? new PagedResult<ServiceModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetPagedAsync error");
                return new PagedResult<ServiceModel>();
            }
        }

        public async Task<ServiceModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/services/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceModel>();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetByIdAsync error");
                return null;
            }
        }

        public async Task<ServiceModel?> CreateAsync(CreateServiceModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/services", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceModel>();
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("CreateAsync failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CreateAsync error");
                return null;
            }
        }

        public async Task<ServiceModel?> UpdateAsync(ServiceModel model)
        {
            try
            {
                var updateDto = new CreateServiceModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    Category = model.Category,
                    IsActive = model.IsActive,
                    ExecutorId = model.ExecutorId
                };

                var response = await _httpClient.PutAsJsonAsync($"api/services/{model.Id}", new
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    Category = model.Category,
                    IsActive = model.IsActive,
                    ExecutorId = model.ExecutorId
                });
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceModel>();
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("UpdateAsync failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "UpdateAsync error");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/services/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "DeleteAsync error");
                return false;
            }
        }

        public async Task<List<ServiceModel>> GetServicesByExecutorAsync(Guid executorId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/services/executor/{executorId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<ServiceModel>>();
                    return result ?? new List<ServiceModel>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogWarning("GetServicesByExecutorAsync: 403 Forbidden - недостаточно прав доступа");
                    throw new UnauthorizedAccessException("Недостаточно прав доступа для просмотра услуг");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("GetServicesByExecutorAsync: 401 Unauthorized - требуется авторизация");
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GetServicesByExecutorAsync failed: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                    return new List<ServiceModel>();
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw; // Пробрасываем дальше
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetServicesByExecutorAsync error");
                throw;
            }
        }

        public async Task<List<ServiceModel>> GetMyServicesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ServiceModel>>("api/services/my-services");
                return result ?? new List<ServiceModel>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetMyServicesAsync error");
                return new List<ServiceModel>();
            }
        }
    }
}

