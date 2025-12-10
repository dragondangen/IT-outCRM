using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace IT_outCRM.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient, 
            ITokenStorage tokenStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _authStateProvider = authStateProvider;
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<AuthResponse?> LoginAsync(LoginModel model)
        {
            try
            {
                Console.WriteLine($"=== LOGIN START ===");
                Console.WriteLine($"Username: {model.Username}");
                Console.WriteLine($"API URL: {_httpClient.BaseAddress}api/auth/login");
                
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
                
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    Console.WriteLine($"Token received: {!string.IsNullOrEmpty(authResponse?.Token)}");
                    
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        Console.WriteLine($"Setting token: {authResponse.Token.Substring(0, Math.Min(10, authResponse.Token.Length))}...");
                        Console.WriteLine($"Token length: {authResponse.Token.Length}");
                        
                        // Сохраняем токен
                        await _tokenStorage.SetTokenAsync(authResponse.Token);
                        
                        // Даем время на сохранение в localStorage
                        await Task.Delay(200);
                        
                        // Verify token was set - проверяем несколько раз
                        string? storedToken = null;
                        for (int i = 0; i < 3; i++)
                        {
                            storedToken = await _tokenStorage.GetTokenAsync();
                            if (storedToken == authResponse.Token)
                            {
                                Console.WriteLine($"Token stored verification: SUCCESS (attempt {i + 1})");
                                break;
                            }
                            Console.WriteLine($"Token stored verification: FAILURE (attempt {i + 1}), retrying...");
                            await Task.Delay(100);
                        }
                        
                        if (storedToken != authResponse.Token)
                        {
                            Console.WriteLine($"CRITICAL: Token verification FAILED after 3 attempts!");
                            Console.WriteLine($"Expected: {authResponse.Token.Substring(0, Math.Min(20, authResponse.Token.Length))}...");
                            Console.WriteLine($"Got: {(storedToken != null ? storedToken.Substring(0, Math.Min(20, storedToken.Length)) : "NULL")}...");
                        }

                        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                        {
                            customProvider.NotifyAuthenticationStateChanged();
                        }
                        
                        Console.WriteLine($"=== LOGIN SUCCESS ===");
                        return authResponse;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response status: {response.StatusCode}");
                    Console.WriteLine($"Error response content: {errorContent}");
                    
                    // Пробуем прочитать как ProblemDetails или просто текст
                    try
                    {
                        var problemDetails = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        if (problemDetails.TryGetProperty("detail", out var detail))
                        {
                            Console.WriteLine($"Error detail: {detail.GetString()}");
                        }
                        if (problemDetails.TryGetProperty("title", out var title))
                        {
                            Console.WriteLine($"Error title: {title.GetString()}");
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Error response (raw): {errorContent}");
                    }
                }
                
                Console.WriteLine($"=== LOGIN FAILED ===");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== LOGIN EXCEPTION ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterModel model)
        {
            try
            {
                Console.WriteLine($"=== REGISTER START ===");
                Console.WriteLine($"Username: {model.Username}");
                Console.WriteLine($"Email: {model.Email}");
                Console.WriteLine($"Role: {model.Role}");
                Console.WriteLine($"Company: {model.CompanyName}");
                Console.WriteLine($"UserType: {model.UserType}");
                Console.WriteLine($"API URL: {_httpClient.BaseAddress}api/auth/register");
                
                // Конвертируем UI модель в DTO для backend
                var registerDto = new RegisterDto
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    CompanyName = model.CompanyName,
                    Inn = model.Inn,
                    LegalForm = model.LegalForm,
                    Phone = model.Phone,
                    UserType = model.UserType
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
                
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    Console.WriteLine($"Token received: {!string.IsNullOrEmpty(authResponse?.Token)}");
                    
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        await _tokenStorage.SetTokenAsync(authResponse.Token);
                        
                        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                        {
                            customProvider.NotifyAuthenticationStateChanged();
                        }
                        
                        Console.WriteLine($"=== REGISTER SUCCESS ===");
                        return authResponse;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                    
                    // Выбрасываем исключение с понятным сообщением
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        throw new InvalidOperationException("Пользователь с таким именем или email уже существует. Попробуйте другие данные.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new InvalidOperationException($"Ошибка регистрации. Проверьте введенные данные. Сервер ответил: {errorContent}");
                    }
                }
                
                Console.WriteLine($"=== REGISTER FAILED ===");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== REGISTER EXCEPTION ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _tokenStorage.RemoveTokenAsync();
            
            if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                customProvider.NotifyAuthenticationStateChanged();
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await _tokenStorage.HasTokenAsync();
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _tokenStorage.GetTokenAsync();
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            try 
            {
                var response = await _httpClient.GetAsync("api/auth/users");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserModel>>();
                    return users ?? new List<UserModel>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AuthService] Failed to get users: {error}");
                    return new List<UserModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception getting users: {ex.Message}");
                return new List<UserModel>();
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try 
            {
                var response = await _httpClient.DeleteAsync($"api/auth/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception deleting user: {ex.Message}");
                return false;
            }
        }

        public async Task<UserModel?> GetCurrentUserAsync()
        {
            try
            {
                // Получаем токен и устанавливаем его перед запросом
                var token = await _tokenStorage.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    SetToken(token);
                }
                
                var response = await _httpClient.GetAsync("api/auth/me");
                Console.WriteLine($"[AuthService] GetCurrentUserAsync response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserModel>();
                    Console.WriteLine($"[AuthService] GetCurrentUserAsync success: {user?.Username ?? "null"}");
                    return user;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AuthService] Failed to get current user: {response.StatusCode} - {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception getting current user: {ex.Message}");
                Console.WriteLine($"[AuthService] StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
