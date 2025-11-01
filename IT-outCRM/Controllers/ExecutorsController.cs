using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Executor;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class ExecutorsController : BaseController
    {
        private readonly IExecutorService _executorService;

        public ExecutorsController(IExecutorService executorService, ILogger<ExecutorsController> logger)
            : base(logger)
        {
            _executorService = executorService;
        }

        /// <summary>
        /// Получить всех исполнителей
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExecutorDto>>> GetAll()
        {
            var executors = await _executorService.GetAllAsync();
            return Ok(executors);
        }

        /// <summary>
        /// Получить исполнителей с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ExecutorDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var executors = await _executorService.GetPagedAsync(pageNumber, pageSize);
            return Ok(executors);
        }

        /// <summary>
        /// Получить исполнителя по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExecutorDto>> GetById(Guid id)
        {
            var executor = await _executorService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<ExecutorDto>(executor, id, "Исполнитель");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(executor);
        }

        /// <summary>
        /// Создать нового исполнителя
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<ExecutorDto>> Create([FromBody] CreateExecutorDto createDto)
        {
            var executor = await _executorService.CreateAsync(createDto);
            LogCreated(executor, executor.Id, "Исполнитель");
            return CreatedAtAction(nameof(GetById), new { id = executor.Id }, executor);
        }

        /// <summary>
        /// Обновить исполнителя
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ExecutorDto>> Update(Guid id, [FromBody] UpdateExecutorDto updateDto)
        {
            var validationResult = ValidateUpdateId<ExecutorDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var executor = await _executorService.UpdateAsync(updateDto);
            LogUpdated(id, "Исполнитель");
            return Ok(executor);
        }

        /// <summary>
        /// Удалить исполнителя
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _executorService.DeleteAsync(id);
            LogDeleted(id, "Исполнитель");
            return NoContent();
        }

        /// <summary>
        /// Получить топ исполнителей по количеству выполненных заказов
        /// </summary>
        [HttpGet("top/{count}")]
        public async Task<ActionResult<IEnumerable<ExecutorDto>>> GetTopExecutors(int count = 10)
        {
            var executors = await _executorService.GetTopExecutorsAsync(count);
            return Ok(executors);
        }
    }
}

