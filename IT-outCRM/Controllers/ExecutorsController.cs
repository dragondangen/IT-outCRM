using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Executor;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExecutorsController : ControllerBase
    {
        private readonly IExecutorService _executorService;
        private readonly ILogger<ExecutorsController> _logger;

        public ExecutorsController(IExecutorService executorService, ILogger<ExecutorsController> logger)
        {
            _executorService = executorService;
            _logger = logger;
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
            if (executor == null)
                return NotFound($"Исполнитель с ID {id} не найден");

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
            _logger.LogInformation("Создан исполнитель с ID: {ExecutorId}", executor.Id);
            return CreatedAtAction(nameof(GetById), new { id = executor.Id }, executor);
        }

        /// <summary>
        /// Обновить исполнителя
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ExecutorDto>> Update(Guid id, [FromBody] UpdateExecutorDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID в URL не совпадает с ID в теле запроса");

            var executor = await _executorService.UpdateAsync(updateDto);
            _logger.LogInformation("Обновлён исполнитель: {ExecutorId}", id);
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
            _logger.LogInformation("Удалён исполнитель: {ExecutorId}", id);
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

