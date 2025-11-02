using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    /// <summary>
    /// Контроллер для управления аккаунтами компаний
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
            : base(logger)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Получить список всех аккаунтов
        /// </summary>
        /// <returns>Список всех аккаунтов в системе</returns>
        /// <remarks>
        /// Доступно для всех авторизованных пользователей (User+)
        /// </remarks>
        /// <response code="200">Список аккаунтов успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        /// <summary>
        /// Получить список аккаунтов с пагинацией
        /// </summary>
        /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице (по умолчанию 10)</param>
        /// <returns>Постраничный список аккаунтов с метаданными</returns>
        /// <remarks>
        /// Пагинация выполняется на уровне базы данных для оптимальной производительности.
        /// 
        /// Пример: GET /api/accounts/paged?pageNumber=1&amp;pageSize=20
        /// </remarks>
        /// <response code="200">Список аккаунтов с метаданными пагинации</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<AccountDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var accounts = await _accountService.GetPagedAsync(pageNumber, pageSize);
            return Ok(accounts);
        }

        /// <summary>
        /// Получить аккаунт по идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор аккаунта (GUID)</param>
        /// <returns>Информация об аккаунте</returns>
        /// <response code="200">Аккаунт найден</response>
        /// <response code="404">Аккаунт не найден</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountDto>> GetById(Guid id)
        {
            var account = await _accountService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<AccountDto>(account, id, "Аккаунт");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(account);
        }

        /// <summary>
        /// Создать новый аккаунт компании
        /// </summary>
        /// <param name="createDto">Данные для создания аккаунта</param>
        /// <returns>Созданный аккаунт</returns>
        /// <remarks>
        /// Доступно только для ролей: Admin, Manager
        /// 
        /// Пример запроса:
        /// 
        ///     POST /api/accounts
        ///     {
        ///        "companyName": "ООО Технологии",
        ///        "foundingDate": "2020-01-15",
        ///        "accountStatusId": "guid-status-id"
        ///     }
        /// </remarks>
        /// <response code="201">Аккаунт успешно создан</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto createDto)
        {
            var account = await _accountService.CreateAsync(createDto);
            LogCreated(account, account.Id, "Аккаунт");
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        /// <summary>
        /// Обновить существующий аккаунт
        /// </summary>
        /// <param name="id">Идентификатор аккаунта</param>
        /// <param name="updateDto">Обновленные данные аккаунта</param>
        /// <returns>Обновленный аккаунт</returns>
        /// <remarks>
        /// Доступно только для ролей: Admin, Manager
        /// 
        /// ID в URL должен совпадать с ID в теле запроса
        /// </remarks>
        /// <response code="200">Аккаунт успешно обновлен</response>
        /// <response code="400">Ошибка валидации или несовпадение ID</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        /// <response code="404">Аккаунт не найден</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountDto>> Update(Guid id, [FromBody] UpdateAccountDto updateDto)
        {
            var validationResult = ValidateUpdateId<AccountDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var account = await _accountService.UpdateAsync(updateDto);
            LogUpdated(id, "Аккаунт");
            return Ok(account);
        }

        /// <summary>
        /// Удалить аккаунт по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор аккаунта для удаления</param>
        /// <remarks>
        /// Доступно только для роли Admin
        /// 
        /// ⚠️ Внимание: Операция необратима!
        /// </remarks>
        /// <response code="204">Аккаунт успешно удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        /// <response code="404">Аккаунт не найден</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _accountService.DeleteAsync(id);
            LogDeleted(id, "Аккаунт");
            return NoContent();
        }

        /// <summary>
        /// Получить аккаунты по статусу (фильтрация)
        /// </summary>
        /// <param name="statusId">Идентификатор статуса аккаунта</param>
        /// <returns>Список аккаунтов с указанным статусом</returns>
        /// <remarks>
        /// Позволяет фильтровать аккаунты по статусу (например: активные, неактивные)
        /// </remarks>
        /// <response code="200">Список отфильтрованных аккаунтов</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet("by-status/{statusId}")]
        [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetByStatus(Guid statusId)
        {
            var accounts = await _accountService.GetAccountsByStatusAsync(statusId);
            return Ok(accounts);
        }
    }
}

