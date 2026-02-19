using IT_outCRM.Application.DTOs.Auth;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateDto);
        Task<UserDto> ToggleUserActiveAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
    }
}
