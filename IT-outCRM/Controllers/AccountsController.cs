using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все аккаунты
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        /// <summary>
        /// Получить аккаунты с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<AccountDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var accounts = await _accountService.GetPagedAsync(pageNumber, pageSize);
            return Ok(accounts);
        }

        /// <summary>
        /// Получить аккаунт по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetById(Guid id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
                return NotFound($"Аккаунт с ID {id} не найден");

            return Ok(account);
        }

        /// <summary>
        /// Создать новый аккаунт
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<AccountDto>> Create([FromBody] CreateAccountDto createDto)
        {
            var account = await _accountService.CreateAsync(createDto);
            _logger.LogInformation("Создан аккаунт: {AccountName}", account.CompanyName);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        /// <summary>
        /// Обновить аккаунт
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<AccountDto>> Update(Guid id, [FromBody] UpdateAccountDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID в URL не совпадает с ID в теле запроса");

            var account = await _accountService.UpdateAsync(updateDto);
            _logger.LogInformation("Обновлён аккаунт: {AccountId}", id);
            return Ok(account);
        }

        /// <summary>
        /// Удалить аккаунт
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _accountService.DeleteAsync(id);
            _logger.LogInformation("Удалён аккаунт: {AccountId}", id);
            return NoContent();
        }

        /// <summary>
        /// Получить аккаунты по статусу
        /// </summary>
        [HttpGet("by-status/{statusId}")]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetByStatus(Guid statusId)
        {
            var accounts = await _accountService.GetAccountsByStatusAsync(statusId);
            return Ok(accounts);
        }
    }
}

