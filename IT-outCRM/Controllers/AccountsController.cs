using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
            : base(logger)
        {
            _accountService = accountService;
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
            var notFoundResult = HandleNotFound<AccountDto>(account, id, "Аккаунт");
            if (notFoundResult != null)
                return notFoundResult;

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
            LogCreated(account, account.Id, "Аккаунт");
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        /// <summary>
        /// Обновить аккаунт
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
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
        /// Удалить аккаунт
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _accountService.DeleteAsync(id);
            LogDeleted(id, "Аккаунт");
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

