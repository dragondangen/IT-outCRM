using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginModel model);
        Task<AuthResponse?> RegisterAsync(RegisterModel model);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<List<UserModel>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(Guid userId);
        void SetToken(string token);
    }
}
