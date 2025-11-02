using IT_outCRM.Application.DTOs.AccountStatus;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    /// <summary>
    /// Контроллер для управления статусами аккаунтов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class AccountStatusesController : BaseController
    {
        private readonly IAccountStatusService _accountStatusService;

        public AccountStatusesController(IAccountStatusService accountStatusService, ILogger<AccountStatusesController> logger)
            : base(logger)
        {
            _accountStatusService = accountStatusService;
        }

        /// <summary>
        /// Получить список всех статусов аккаунтов
        /// </summary>
        /// <returns>Список всех статусов</returns>
        /// <remarks>
        /// Доступно для всех авторизованных пользователей
        /// </remarks>
        /// <response code="200">Список статусов успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccountStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountStatusDto>>> GetAll()
        {
            var statuses = await _accountStatusService.GetAllAsync();
            return Ok(statuses);
        }

        /// <summary>
        /// Получить список статусов с пагинацией
        /// </summary>
        /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 10)</param>
        /// <returns>Постраничный список статусов</returns>
        /// <response code="200">Список статусов с метаданными пагинации</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<AccountStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<AccountStatusDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var statuses = await _accountStatusService.GetPagedAsync(pageNumber, pageSize);
            return Ok(statuses);
        }

        /// <summary>
        /// Получить статус по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор статуса (GUID)</param>
        /// <returns>Информация о статусе</returns>
        /// <response code="200">Статус найден</response>
        /// <response code="404">Статус не найден</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountStatusDto>> GetById(Guid id)
        {
            var status = await _accountStatusService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<AccountStatusDto>(status, id, "Статус аккаунта");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(status);
        }

        /// <summary>
        /// Создать новый статус аккаунта
        /// </summary>
        /// <param name="createDto">Данные для создания статуса</param>
        /// <returns>Созданный статус</returns>
        /// <remarks>
        /// Доступно только для ролей: Admin, Manager
        /// 
        /// Пример запроса:
        /// 
        ///     POST /api/accountstatuses
        ///     {
        ///        "name": "Активный"
        ///     }
        /// </remarks>
        /// <response code="201">Статус успешно создан</response>
        /// <response code="400">Ошибка валидации данных или статус с таким названием уже существует</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountStatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AccountStatusDto>> Create([FromBody] CreateAccountStatusDto createDto)
        {
            var status = await _accountStatusService.CreateAsync(createDto);
            LogCreated(status, status.Id, "Статус аккаунта");
            return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);
        }

        /// <summary>
        /// Обновить существующий статус
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <param name="updateDto">Обновленные данные статуса</param>
        /// <returns>Обновленный статус</returns>
        /// <remarks>
        /// Доступно только для ролей: Admin, Manager
        /// 
        /// ID в URL должен совпадать с ID в теле запроса
        /// </remarks>
        /// <response code="200">Статус успешно обновлен</response>
        /// <response code="400">Ошибка валидации или несовпадение ID</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        /// <response code="404">Статус не найден</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AccountStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountStatusDto>> Update(Guid id, [FromBody] UpdateAccountStatusDto updateDto)
        {
            var validationResult = ValidateUpdateId<AccountStatusDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var status = await _accountStatusService.UpdateAsync(updateDto);
            LogUpdated(id, "Статус аккаунта");
            return Ok(status);
        }

        /// <summary>
        /// Удалить статус по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса для удаления</param>
        /// <remarks>
        /// Доступно только для роли Admin
        /// 
        /// ⚠️ Внимание: Операция необратима! Убедитесь, что статус не используется в аккаунтах.
        /// </remarks>
        /// <response code="204">Статус успешно удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        /// <response code="404">Статус не найден</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _accountStatusService.DeleteAsync(id);
            LogDeleted(id, "Статус аккаунта");
            return NoContent();
        }
    }
}

