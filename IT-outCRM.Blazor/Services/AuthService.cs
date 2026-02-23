using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            HttpClient httpClient, 
            ITokenStorage tokenStorage,
            AuthenticationStateProvider authStateProvider,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _authStateProvider = authStateProvider;
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

        public async Task<AuthResponse?> LoginAsync(LoginModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        await _tokenStorage.SetTokenAsync(authResponse.Token);
                        await Task.Delay(200);
                        
                        for (int i = 0; i < 3; i++)
                        {
                            var storedToken = await _tokenStorage.GetTokenAsync();
                            if (storedToken == authResponse.Token) break;
                            await Task.Delay(100);
                        }

                        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                        {
                            customProvider.NotifyAuthenticationStateChanged();
                        }
                        
                        return authResponse;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Login failed");
                return null;
            }
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterModel model)
        {
            try
            {
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

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        await _tokenStorage.SetTokenAsync(authResponse.Token);
                        
                        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
                        {
                            customProvider.NotifyAuthenticationStateChanged();
                        }
                        
                        return authResponse;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        throw new InvalidOperationException("Пользователь с таким именем или email уже существует. Попробуйте другие данные.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new InvalidOperationException($"Ошибка регистрации. Проверьте введенные данные. Сервер ответил: {errorContent}");
                    }
                }
                
                return null;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Сервер недоступен. Попробуйте позже. ({ex.Message})");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка регистрации: {ex.Message}");
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
                    return new List<UserModel>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch users list");
                return new List<UserModel>();
            }
        }

        public async Task<UserModel?> UpdateUserAsync(Guid userId, UpdateUserModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/auth/users/{userId}", model);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<UserModel>();

                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(error);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to update user {UserId}", userId); return null; }
        }

        public async Task<UserModel?> ToggleUserActiveAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/auth/users/{userId}/toggle-active", null);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                return null;
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to toggle user {UserId}", userId); return null; }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try 
            {
                var response = await _httpClient.DeleteAsync($"api/auth/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete user {UserId}", userId); return false; }
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
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }
                return null;
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Failed to get current user"); return null; }
        }
    }
}
