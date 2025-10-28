using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Customer;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Получить всех клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Получить клиентов с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var customers = await _customerService.GetPagedAsync(pageNumber, pageSize);
            return Ok(customers);
        }

        /// <summary>
        /// Получить клиента по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Клиент с ID {id} не найден");

            return Ok(customer);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto createDto)
        {
            var customer = await _customerService.CreateAsync(createDto);
            _logger.LogInformation("Создан клиент с ID: {CustomerId}", customer.Id);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID в URL не совпадает с ID в теле запроса");

            var customer = await _customerService.UpdateAsync(updateDto);
            _logger.LogInformation("Обновлён клиент: {CustomerId}", id);
            return Ok(customer);
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _customerService.DeleteAsync(id);
            _logger.LogInformation("Удалён клиент: {CustomerId}", id);
            return NoContent();
        }

        /// <summary>
        /// Получить клиентов по компании
        /// </summary>
        [HttpGet("by-company/{companyId}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetByCompany(Guid companyId)
        {
            var customers = await _customerService.GetCustomersByCompanyAsync(companyId);
            return Ok(customers);
        }
    }
}

