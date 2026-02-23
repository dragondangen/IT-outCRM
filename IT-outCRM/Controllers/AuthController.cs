using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace IT_outCRM.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации и авторизации пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового пользователя в системе
        /// </summary>
        /// <param name="registerDto">Данные для регистрации (username, email, password, role)</param>
        /// <returns>JWT токен и информация о пользователе</returns>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST /api/auth/register
        ///     {
        ///        "username": "newuser",
        ///        "email": "user@example.com",
        ///        "password": "SecurePassword123!",
        ///        "role": "User"
        ///     }
        ///     
        /// Доступные роли: Admin, Manager, User
        /// </remarks>
        /// <response code="200">Пользователь успешно зарегистрирован</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="429">Слишком много попыток регистрации</response>
        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            _logger.LogInformation("Пользователь {Username} успешно зарегистрирован", registerDto.Username);
            return Ok(response);
        }

        /// <summary>
        /// Аутентификация пользователя (вход в систему)
        /// </summary>
        /// <param name="loginDto">Учетные данные (username, password)</param>
        /// <returns>JWT токен для авторизации последующих запросов</returns>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST /api/auth/login
        ///     {
        ///        "username": "admin",
        ///        "password": "Admin123!"
        ///     }
        ///     
        /// Полученный токен используйте в заголовке: Authorization: Bearer {token}
        /// </remarks>
        /// <response code="200">Успешная аутентификация</response>
        /// <response code="401">Неверные учетные данные</response>
        /// <response code="429">Слишком много попыток входа</response>
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            _logger.LogInformation("Пользователь {Username} успешно вошёл в систему", loginDto.Username);
            return Ok(response);
        }

        /// <summary>
        /// Получить информацию о текущем авторизованном пользователе
        /// </summary>
        /// <returns>Информация о пользователе</returns>
        /// <remarks>
        /// Требуется JWT токен в заголовке Authorization
        /// </remarks>
        /// <response code="200">Информация о пользователе</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Пользователь не найден</response>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userGuid);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Получить список всех пользователей системы
        /// </summary>
        /// <returns>Список всех пользователей</returns>
        /// <remarks>
        /// Доступно только пользователям с ролью Admin
        /// </remarks>
        /// <response code="200">Список пользователей</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Удалить (деактивировать) пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Доступно только пользователям с ролью Admin
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var user = await _authService.UpdateUserAsync(id, updateDto);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id}/toggle-active")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> ToggleUserActive(Guid id)
        {
            try
            {
                var user = await _authService.ToggleUserActiveAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try 
            {
                await _authService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Запрос сброса пароля — возвращает токен (в production отправлялся бы по email)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("request-reset")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestResetDto dto)
        {
            try
            {
                var token = await _authService.RequestPasswordResetAsync(dto.Email);
                _logger.LogInformation("Password reset requested for {Email}", dto.Email);
                return Ok(new { message = "Токен сброса создан", token });
            }
            catch (KeyNotFoundException)
            {
                return Ok(new { message = "Если пользователь существует, ему будет отправлена ссылка" });
            }
        }

        /// <summary>
        /// Сброс пароля по токену
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
                return Ok(new { message = "Пароль успешно изменён" });
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Недействительный или просроченный токен сброса");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class RequestResetDto
    {
        public string Email { get; set; } = "";
    }

    public class ResetPasswordDto
    {
        public string Token { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }
}
