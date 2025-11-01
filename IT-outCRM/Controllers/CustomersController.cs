using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Customer;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
            : base(logger)
        {
            _customerService = customerService;
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
            var notFoundResult = HandleNotFound<CustomerDto>(customer, id, "Клиент");
            if (notFoundResult != null)
                return notFoundResult;

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
            LogCreated(customer, customer.Id, "Клиент");
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
        {
            var validationResult = ValidateUpdateId<CustomerDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var customer = await _customerService.UpdateAsync(updateDto);
            LogUpdated(id, "Клиент");
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
            LogDeleted(id, "Клиент");
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

