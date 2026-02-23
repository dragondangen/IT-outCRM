using System.Security.Claims;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(IUnitOfWork unitOfWork, IWebHostEnvironment environment, ILogger<ProfileController> logger)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet("my-info")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMyInfo()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            if (string.IsNullOrEmpty(userEmail))
                return Ok(new { });

            try
            {
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null) return Ok(new { });

                var company = await _unitOfWork.Companies.GetByContactPersonIdAsync(contact.Id);
                if (company == null) return Ok(new { contactPerson = new { contact.FirstName, contact.LastName, contact.PhoneNumber, contact.Role } });

                string? accountName = null;
                string? accountStatus = null;
                Guid? customerId = null;
                Guid? executorId = null;
                int completedOrders = 0;

                if (userRole == "Executor")
                {
                    var executor = await _unitOfWork.Executors.GetByCompanyIdAsync(company.Id);
                    if (executor != null)
                    {
                        executorId = executor.Id;
                        completedOrders = executor.CompletedOrders;
                        var account = await _unitOfWork.Accounts.GetByIdAsync(executor.AccountId);
                        if (account != null)
                        {
                            accountName = account.CompanyName;
                            var status = await _unitOfWork.AccountStatuses.GetByIdAsync(account.AccountStatusId);
                            accountStatus = status?.Name;
                        }
                    }
                }
                else
                {
                    var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                    var customer = customers.FirstOrDefault();
                    if (customer != null)
                    {
                        customerId = customer.Id;
                        var account = await _unitOfWork.Accounts.GetByIdAsync(customer.AccountId);
                        if (account != null)
                        {
                            accountName = account.CompanyName;
                            var status = await _unitOfWork.AccountStatuses.GetByIdAsync(account.AccountStatusId);
                            accountStatus = status?.Name;
                        }
                    }
                }

                return Ok(new
                {
                    companyName = company.Name,
                    companyInn = company.Inn,
                    companyLegalForm = company.LegalForm,
                    contactFirstName = contact.FirstName,
                    contactLastName = contact.LastName,
                    contactPhone = contact.PhoneNumber,
                    contactRole = contact.Role,
                    accountName,
                    accountStatus,
                    customerId,
                    executorId,
                    completedOrders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile info for {Email}", userEmail);
                return Ok(new { });
            }
        }

        /// <summary>
        /// Загрузить аватар для текущего пользователя
        /// </summary>
        /// <param name="file">Файл изображения (JPG, PNG, до 5MB)</param>
        /// <returns>URL загруженного аватара</returns>
        /// <remarks>
        /// Загружает изображение аватара для текущего авторизованного пользователя.
        /// Поддерживаемые форматы: JPG, JPEG, PNG, GIF
        /// Максимальный размер файла: 5MB
        /// </remarks>
        /// <response code="200">Аватар успешно загружен</response>
        /// <response code="400">Ошибка: неверный формат файла или превышен размер</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpPost("avatar")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UploadAvatar(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Пользователь не найден");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }

            // Проверка размера файла (5MB)
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest("Размер файла не должен превышать 5MB");
            }

            // Проверка формата файла
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Поддерживаются только форматы: JPG, JPEG, PNG, GIF");
            }

            try
            {
                // Получаем пользователя
                var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
                if (user == null)
                {
                    return NotFound("Пользователь не найден");
                }

                // Создаем директорию для аватаров в wwwroot
                var webRoot = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRoot))
                {
                    // Если wwwroot не существует, создаем его
                    webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    if (!Directory.Exists(webRoot))
                    {
                        Directory.CreateDirectory(webRoot);
                    }
                }
                
                var avatarsDirectory = Path.Combine(webRoot, "avatars");
                if (!Directory.Exists(avatarsDirectory))
                {
                    Directory.CreateDirectory(avatarsDirectory);
                }

                // Удаляем старый аватар, если он существует
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var webRootForDelete = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                    var oldAvatarPath = Path.Combine(webRootForDelete, user.AvatarUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldAvatarPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldAvatarPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Не удалось удалить старый аватар: {Path}", oldAvatarPath);
                        }
                    }
                }

                // Генерируем уникальное имя файла
                var fileName = $"{userGuid}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(avatarsDirectory, fileName);

                _logger.LogInformation("Сохранение аватара: {FilePath}, WebRoot: {WebRoot}", filePath, webRoot);

                // Сохраняем файл
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Проверяем, что файл действительно сохранен
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError("Файл не был сохранен: {FilePath}", filePath);
                    return StatusCode(500, "Ошибка при сохранении файла");
                }

                // Обновляем URL аватара в базе данных
                var avatarUrl = $"/avatars/{fileName}";
                user.AvatarUrl = avatarUrl;
                
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Аватар загружен для пользователя {UserId}: {AvatarUrl}, Путь: {FilePath}", userGuid, avatarUrl, filePath);

                return Ok(new { avatarUrl = avatarUrl, message = "Аватар успешно загружен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке аватара для пользователя {UserId}", userGuid);
                return StatusCode(500, "Ошибка при загрузке аватара");
            }
        }

        /// <summary>
        /// Удалить аватар текущего пользователя
        /// </summary>
        /// <returns>Результат операции</returns>
        /// <response code="200">Аватар успешно удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpDelete("avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteAvatar()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Пользователь не найден");
            }

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userGuid);
                if (user == null)
                {
                    return NotFound("Пользователь не найден");
                }

                // Удаляем файл аватара, если он существует
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
                    var avatarPath = Path.Combine(webRoot, user.AvatarUrl.TrimStart('/'));
                    if (System.IO.File.Exists(avatarPath))
                    {
                        try
                        {
                            System.IO.File.Delete(avatarPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Не удалось удалить файл аватара: {Path}", avatarPath);
                        }
                    }

                    // Очищаем URL в базе данных
                    user.AvatarUrl = null;
                    await _unitOfWork.Users.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }

                return Ok(new { message = "Аватар успешно удален" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении аватара для пользователя {UserId}", userGuid);
                return StatusCode(500, "Ошибка при удалении аватара");
            }
        }
    }
}

