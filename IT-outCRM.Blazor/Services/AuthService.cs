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
                        await _tokenStorage.SetTokenAsync(authResponse.Token);
                        
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
                    Console.WriteLine($"Error response: {errorContent}");
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
                Console.WriteLine($"API URL: {_httpClient.BaseAddress}api/auth/register");
                
                // Конвертируем UI модель в DTO для backend
                var registerDto = new RegisterDto
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
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
                        throw new InvalidOperationException("Проверьте правильность введенных данных. Пароль должен содержать минимум 8 символов, заглавную букву, строчную букву, цифру и спецсимвол.");
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
    }
}

