using System.Security.Claims;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Service;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ServicesController : BaseController
    {
        private readonly IServiceService _serviceService;
        private readonly IUnitOfWork _unitOfWork;

        public ServicesController(IServiceService serviceService, IUnitOfWork unitOfWork, ILogger<ServicesController> logger)
            : base(logger)
        {
            _serviceService = serviceService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Получить список всех услуг
        /// </summary>
        /// <returns>Список всех услуг</returns>
        /// <response code="200">Список услуг успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll()
        {
            var services = await _serviceService.GetAllAsync();
            return Ok(services);
        }

        [HttpGet("my-services")]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetMyServices()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Ok(Enumerable.Empty<ServiceDto>());

            try
            {
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null) return Ok(Enumerable.Empty<ServiceDto>());

                var company = await _unitOfWork.Companies.GetByContactPersonIdAsync(contact.Id);
                if (company == null) return Ok(Enumerable.Empty<ServiceDto>());

                var executor = await _unitOfWork.Executors.GetByCompanyIdAsync(company.Id);
                if (executor == null) return Ok(Enumerable.Empty<ServiceDto>());

                var services = await _serviceService.GetServicesByExecutorAsync(executor.Id);
                return Ok(services);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting my services for {Email}", userEmail);
                return Ok(Enumerable.Empty<ServiceDto>());
            }
        }

        /// <summary>
        /// Получить список услуг с пагинацией
        /// </summary>
        /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 10)</param>
        /// <returns>Постраничный список услуг</returns>
        /// <response code="200">Список услуг с метаданными пагинации</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<ServiceDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var services = await _serviceService.GetPagedAsync(pageNumber, pageSize);
            return Ok(services);
        }

        /// <summary>
        /// Получить услугу по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор услуги (GUID)</param>
        /// <returns>Информация об услуге</returns>
        /// <response code="200">Услуга найдена</response>
        /// <response code="404">Услуга не найдена</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceDto>> GetById(Guid id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<ServiceDto>(service, id, "Услуга");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(service);
        }

        /// <summary>
        /// Получить услуги по исполнителю
        /// </summary>
        /// <param name="executorId">Идентификатор исполнителя</param>
        /// <returns>Список услуг исполнителя</returns>
        /// <response code="200">Список услуг успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("executor/{executorId}")]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetByExecutor(Guid executorId)
        {
            var services = await _serviceService.GetServicesByExecutorAsync(executorId);
            return Ok(services);
        }

        /// <summary>
        /// Получить активные услуги
        /// </summary>
        /// <returns>Список активных услуг</returns>
        /// <response code="200">Список активных услуг успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetActive()
        {
            var services = await _serviceService.GetActiveServicesAsync();
            return Ok(services);
        }

        /// <summary>
        /// Создать новую услугу
        /// </summary>
        /// <param name="createDto">Данные для создания услуги</param>
        /// <returns>Созданная услуга</returns>
        /// <remarks>
        /// Доступно для ролей: Executor (только свои услуги), Admin, Manager
        /// </remarks>
        /// <response code="201">Услуга успешно создана</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ServiceDto>> Create([FromBody] CreateServiceDto createDto)
        {
            var service = await _serviceService.CreateAsync(createDto);
            LogCreated(service, service.Id, "Услуга");
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        /// <summary>
        /// Обновить существующую услугу
        /// </summary>
        /// <param name="id">Идентификатор услуги</param>
        /// <param name="updateDto">Данные для обновления услуги</param>
        /// <returns>Обновленная услуга</returns>
        /// <remarks>
        /// Доступно для ролей: Executor (только свои услуги), Admin, Manager
        /// </remarks>
        /// <response code="200">Услуга успешно обновлена</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="404">Услуга не найдена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ServiceDto>> Update(Guid id, [FromBody] UpdateServiceDto updateDto)
        {
            var validationResult = ValidateUpdateId<ServiceDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var service = await _serviceService.UpdateAsync(updateDto);
            LogUpdated(id, "Услуга");
            return Ok(service);
        }

        /// <summary>
        /// Удалить услугу
        /// </summary>
        /// <param name="id">Идентификатор услуги</param>
        /// <returns>Результат удаления</returns>
        /// <remarks>
        /// Доступно для ролей: Executor (только свои услуги), Admin, Manager
        /// </remarks>
        /// <response code="204">Услуга успешно удалена</response>
        /// <response code="404">Услуга не найдена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _serviceService.DeleteAsync(id);
            LogDeleted(id, "Услуга");
            return NoContent();
        }
    }
}

