using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
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

        public async Task<List<OrderModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderModel>>("api/orders");
                return result ?? new List<OrderModel>();
            }
            catch
            {
                return new List<OrderModel>();
            }
        }

        public async Task<PagedResult<OrderModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<PagedResult<OrderModel>>(
                    $"api/orders/paged?pageNumber={pageNumber}&pageSize={pageSize}");
                return result ?? new PagedResult<OrderModel>();
            }
            catch
            {
                return new PagedResult<OrderModel>();
            }
        }

        public async Task<OrderModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderModel>();
                }
                
                // Логируем ошибку для отладки
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[OrderService] GetById failed: {response.StatusCode} - {errorContent}");
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderService] Exception in GetById: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderModel?> CreateAsync(CreateOrderModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderModel>();
                }
                
                // Читаем сообщение об ошибке из ответа
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = "Ошибка при создании заказа";
                
                // Если ответ пустой, используем дефолтное сообщение
                if (string.IsNullOrWhiteSpace(errorContent))
                {
                    errorMessage = $"Ошибка сервера ({response.StatusCode})";
                }
                // Пытаемся распарсить JSON ошибки
                else if (errorContent.TrimStart().StartsWith('{') || errorContent.TrimStart().StartsWith('['))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(errorContent);
                        var root = doc.RootElement;
                        
                        // Проверяем формат с массивом errors (валидационные ошибки)
                        if (root.TryGetProperty("errors", out var errorsElement))
                        {
                            var errorList = new List<string>();
                            if (errorsElement.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var error in errorsElement.EnumerateArray())
                                {
                                    var errorText = error.ValueKind == JsonValueKind.String 
                                        ? error.GetString() 
                                        : error.ToString();
                                    if (!string.IsNullOrWhiteSpace(errorText))
                                    {
                                        errorList.Add(errorText);
                                    }
                                }
                            }
                            else if (errorsElement.ValueKind == JsonValueKind.Object)
                            {
                                // Формат: {"errors": {"Field": ["Error1", "Error2"]}}
                                foreach (var property in errorsElement.EnumerateObject())
                                {
                                    if (property.Value.ValueKind == JsonValueKind.Array)
                                    {
                                        foreach (var error in property.Value.EnumerateArray())
                                        {
                                            var errorText = error.ValueKind == JsonValueKind.String 
                                                ? error.GetString() 
                                                : error.ToString();
                                            if (!string.IsNullOrWhiteSpace(errorText))
                                            {
                                                errorList.Add($"{property.Name}: {errorText}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var errorText = property.Value.ValueKind == JsonValueKind.String 
                                            ? property.Value.GetString() 
                                            : property.Value.ToString();
                                        if (!string.IsNullOrWhiteSpace(errorText))
                                        {
                                            errorList.Add($"{property.Name}: {errorText}");
                                        }
                                    }
                                }
                            }
                            
                            if (errorList.Any())
                            {
                                errorMessage = string.Join(". ", errorList);
                            }
                        }
                        // Проверяем формат ErrorResponse (из middleware)
                        else if (root.TryGetProperty("message", out var messageElement))
                        {
                            errorMessage = messageElement.ValueKind == JsonValueKind.String 
                                ? messageElement.GetString() ?? errorMessage 
                                : messageElement.ToString();
                            
                            if (root.TryGetProperty("details", out var detailsElement) && detailsElement.ValueKind == JsonValueKind.String)
                            {
                                var details = detailsElement.GetString();
                                if (!string.IsNullOrEmpty(details))
                                {
                                    errorMessage += $". {details}";
                                }
                            }
                        }
                        // Проверяем ProblemDetails формат
                        else if (root.TryGetProperty("title", out var titleElement))
                        {
                            errorMessage = titleElement.ValueKind == JsonValueKind.String 
                                ? titleElement.GetString() ?? errorMessage 
                                : titleElement.ToString();
                        }
                    }
                    catch (JsonException)
                    {
                        // Если не удалось распарсить JSON, используем исходный текст
                        if (errorContent.Length < 500)
                        {
                            errorMessage = errorContent.Trim();
                        }
                    }
                }
                else
                {
                    // Простой текстовый ответ
                    errorMessage = errorContent.Trim().Trim('"');
                }
                
                throw new InvalidOperationException(errorMessage);
            }
            catch (InvalidOperationException)
            {
                // Пробрасываем уже созданные исключения с сообщениями об ошибках
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при создании заказа: {ex.Message}", ex);
            }
        }

        public async Task<OrderModel?> UpdateAsync(OrderModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orders/{model.Id}", model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OrderModel>();
                }
                
                // Читаем сообщение об ошибке из ответа
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = "Ошибка при обновлении заказа";
                
                // Если ответ пустой, используем дефолтное сообщение
                if (string.IsNullOrWhiteSpace(errorContent))
                {
                    errorMessage = $"Ошибка сервера ({response.StatusCode})";
                }
                // Пытаемся распарсить JSON ошибки
                else if (errorContent.TrimStart().StartsWith('{') || errorContent.TrimStart().StartsWith('['))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(errorContent);
                        var root = doc.RootElement;
                        
                        // Проверяем формат с массивом errors (валидационные ошибки)
                        if (root.TryGetProperty("errors", out var errorsElement))
                        {
                            var errorList = new List<string>();
                            if (errorsElement.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var error in errorsElement.EnumerateArray())
                                {
                                    var errorText = error.ValueKind == JsonValueKind.String 
                                        ? error.GetString() 
                                        : error.ToString();
                                    if (!string.IsNullOrWhiteSpace(errorText))
                                    {
                                        errorList.Add(errorText);
            }
                                }
                            }
                            else if (errorsElement.ValueKind == JsonValueKind.Object)
                            {
                                // Формат: {"errors": {"Field": ["Error1", "Error2"]}}
                                foreach (var property in errorsElement.EnumerateObject())
                                {
                                    if (property.Value.ValueKind == JsonValueKind.Array)
                                    {
                                        foreach (var error in property.Value.EnumerateArray())
                                        {
                                            var errorText = error.ValueKind == JsonValueKind.String 
                                                ? error.GetString() 
                                                : error.ToString();
                                            if (!string.IsNullOrWhiteSpace(errorText))
                                            {
                                                errorList.Add($"{property.Name}: {errorText}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var errorText = property.Value.ValueKind == JsonValueKind.String 
                                            ? property.Value.GetString() 
                                            : property.Value.ToString();
                                        if (!string.IsNullOrWhiteSpace(errorText))
                                        {
                                            errorList.Add($"{property.Name}: {errorText}");
                                        }
                                    }
                                }
                            }
                            
                            if (errorList.Any())
                            {
                                errorMessage = string.Join(". ", errorList);
                            }
                        }
                        // Проверяем формат ErrorResponse (из middleware)
                        else if (root.TryGetProperty("message", out var messageElement))
                        {
                            errorMessage = messageElement.ValueKind == JsonValueKind.String 
                                ? messageElement.GetString() ?? errorMessage 
                                : messageElement.ToString();
                            
                            if (root.TryGetProperty("details", out var detailsElement) && detailsElement.ValueKind == JsonValueKind.String)
                            {
                                var details = detailsElement.GetString();
                                if (!string.IsNullOrEmpty(details))
                                {
                                    errorMessage += $". {details}";
                                }
                            }
                        }
                        // Проверяем ProblemDetails формат
                        else if (root.TryGetProperty("title", out var titleElement))
                        {
                            errorMessage = titleElement.ValueKind == JsonValueKind.String 
                                ? titleElement.GetString() ?? errorMessage 
                                : titleElement.ToString();
                        }
                    }
                    catch (JsonException)
                    {
                        // Если не удалось распарсить JSON, используем исходный текст
                        if (errorContent.Length < 500)
                        {
                            errorMessage = errorContent.Trim();
                        }
                    }
                }
                else
                {
                    // Простой текстовый ответ
                    errorMessage = errorContent.Trim().Trim('"');
                }
                
                throw new InvalidOperationException(errorMessage);
            }
            catch (InvalidOperationException)
            {
                // Пробрасываем уже созданные исключения с сообщениями об ошибках
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при обновлении заказа: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/orders/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<OrderModel>> GetByCustomerAsync(Guid customerId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderModel>>(
                    $"api/orders/by-customer/{customerId}");
                return result ?? new List<OrderModel>();
            }
            catch
            {
                return new List<OrderModel>();
            }
        }

        public async Task<List<OrderModel>> GetMyOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/orders/my-orders");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<OrderModel>>();
                    Console.WriteLine($"[OrderService] GetMyOrdersAsync: Successfully loaded {result?.Count ?? 0} orders");
                    return result ?? new List<OrderModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[OrderService] GetMyOrdersAsync failed: {response.StatusCode} - {errorContent}");
                    
                    // Пытаемся извлечь сообщение об ошибке
                    string errorMessage = "Ошибка при получении заказов";
                    
                    if (!string.IsNullOrWhiteSpace(errorContent))
                    {
                        try
                        {
                            // Пробуем распарсить как JSON (если это ErrorResponse)
                            if (errorContent.TrimStart().StartsWith('{'))
                            {
                                using var doc = JsonDocument.Parse(errorContent);
                                var root = doc.RootElement;
                                
                                if (root.TryGetProperty("message", out var messageElement))
                                {
                                    errorMessage = messageElement.ValueKind == JsonValueKind.String 
                                        ? messageElement.GetString() ?? errorMessage 
                                        : messageElement.ToString();
                                    
                                    if (root.TryGetProperty("details", out var detailsElement) && detailsElement.ValueKind == JsonValueKind.String)
                                    {
                                        var details = detailsElement.GetString();
                                        if (!string.IsNullOrEmpty(details))
                                        {
                                            errorMessage += $". {details}";
                                        }
                                    }
                                }
                                else if (root.TryGetProperty("title", out var titleElement))
                                {
                                    errorMessage = titleElement.ValueKind == JsonValueKind.String 
                                        ? titleElement.GetString() ?? errorMessage 
                                        : titleElement.ToString();
                                }
                            }
                            else
                            {
                                // Простая строка - убираем кавычки, если есть
                                errorMessage = errorContent.Trim().Trim('"');
                            }
                        }
                        catch (JsonException)
                        {
                            // Если не JSON, используем как простую строку
                            errorMessage = errorContent.Trim().Trim('"');
                        }
                    }
                    
                    throw new InvalidOperationException(errorMessage);
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderService] Error in GetMyOrdersAsync: {ex.Message}");
                throw new InvalidOperationException($"Ошибка при получении заказов: {ex.Message}", ex);
            }
        }

        public async Task<bool> TakeOrderAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/orders/{id}/take", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
