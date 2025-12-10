using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class ServiceService : IServiceService
    {
        private readonly HttpClient _httpClient;

        public ServiceService(HttpClient httpClient)
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

        public async Task<List<ServiceModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ServiceModel>>("api/services");
                return result ?? new List<ServiceModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceService] GetAllAsync error: {ex.Message}");
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
                Console.WriteLine($"[ServiceService] GetPagedAsync error: {ex.Message}");
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
                Console.WriteLine($"[ServiceService] GetByIdAsync error: {ex.Message}");
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
                Console.WriteLine($"[ServiceService] CreateAsync failed: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceService] CreateAsync error: {ex.Message}");
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
                Console.WriteLine($"[ServiceService] UpdateAsync failed: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceService] UpdateAsync error: {ex.Message}");
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
                Console.WriteLine($"[ServiceService] DeleteAsync error: {ex.Message}");
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
                    Console.WriteLine($"[ServiceService] GetServicesByExecutorAsync: 403 Forbidden - недостаточно прав доступа");
                    throw new UnauthorizedAccessException("Недостаточно прав доступа для просмотра услуг");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine($"[ServiceService] GetServicesByExecutorAsync: 401 Unauthorized - требуется авторизация");
                    throw new UnauthorizedAccessException("Требуется авторизация");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ServiceService] GetServicesByExecutorAsync failed: {response.StatusCode} - {errorContent}");
                    return new List<ServiceModel>();
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw; // Пробрасываем дальше
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceService] GetServicesByExecutorAsync error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<ServiceModel>> GetMyServicesAsync()
        {
            try
            {
                // Получаем услуги текущего пользователя (исполнителя)
                // Для этого нужно получить ExecutorId текущего пользователя
                // Пока используем GetAllAsync и фильтруем на клиенте
                // В будущем можно добавить endpoint /api/services/my
                var allServices = await GetAllAsync();
                return allServices; // Пока возвращаем все, фильтрация будет на уровне UI
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceService] GetMyServicesAsync error: {ex.Message}");
                return new List<ServiceModel>();
            }
        }
    }
}

