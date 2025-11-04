using AutoMapper;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;
using Microsoft.Extensions.Configuration;

namespace IT_outCRM.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Проверка на существование пользователя
            // Используем общее сообщение для предотвращения перебора имен пользователей
            if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
                throw new InvalidOperationException("Не удалось завершить регистрацию. Проверьте введенные данные.");

            if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
                throw new InvalidOperationException("Не удалось завершить регистрацию. Проверьте введенные данные.");

            // Создание пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Генерация токена
            var token = _jwtService.GenerateToken(user);
            var expirationHours = Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24");

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Поиск пользователя
            var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username)
                ?? throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");

            // Проверка пароля
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");

            // Проверка активности
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Учетная запись деактивирована");

            // Обновление времени последнего входа
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Генерация токена
            var token = _jwtService.GenerateToken(user);
            var expirationHours = Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24");

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}

