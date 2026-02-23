using System.Security.Claims;
using IT_outCRM.Application.DTOs.Deal;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class DealsController : BaseController
    {
        private readonly IDealService _dealService;
        private readonly IUnitOfWork _unitOfWork;

        public DealsController(IDealService dealService, IUnitOfWork unitOfWork, ILogger<DealsController> logger)
            : base(logger)
        {
            _dealService = dealService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetAll()
        {
            var deals = await _dealService.GetAllAsync();
            return Ok(deals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DealDto>> GetById(Guid id)
        {
            var deal = await _dealService.GetDealWithDetailsAsync(id);
            if (deal == null) return NotFound($"Сделка с ID {id} не найдена");
            return Ok(deal);
        }

        [HttpGet("my-deals")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetMyDeals()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
                return Ok(await _dealService.GetAllAsync());

            try
            {
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null) return BadRequest("Контактное лицо не найдено");

                var company = await _unitOfWork.Companies.GetByContactPersonIdAsync(contact.Id);
                if (company == null) return BadRequest("Компания не найдена");

                if (User.IsInRole("Executor"))
                {
                    var executor = await _unitOfWork.Executors.GetByCompanyIdAsync(company.Id);
                    if (executor == null) return BadRequest("Профиль исполнителя не найден");
                    return Ok(await _dealService.GetDealsByExecutorAsync(executor.Id));
                }

                var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                var customer = customers.FirstOrDefault();
                if (customer == null) return BadRequest("Профиль клиента не найден");
                return Ok(await _dealService.GetDealsByCustomerAsync(customer.Id));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting deals for user {Email}", userEmail);
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DealDto>> Create([FromBody] CreateDealDto createDto)
        {
            try
            {
                var deal = await _dealService.CreateAsync(createDto);
                LogCreated(deal, deal.Id, "Сделка");
                return CreatedAtAction(nameof(GetById), new { id = deal.Id }, deal);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating deal");
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        [HttpPost("from-order")]
        [Authorize]
        public async Task<ActionResult<DealDto>> CreateFromOrder([FromBody] CreateDealFromOrderRequest request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            try
            {
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null) return BadRequest("Контактное лицо не найдено");

                var company = await _unitOfWork.Companies.GetByContactPersonIdAsync(contact.Id);
                if (company == null) return BadRequest("Компания не найдена");

                Guid customerId;
                Guid executorId;

                if (User.IsInRole("Executor"))
                {
                    var executor = await _unitOfWork.Executors.GetByCompanyIdAsync(company.Id);
                    if (executor == null) return BadRequest("Профиль исполнителя не найден");
                    executorId = executor.Id;
                    customerId = request.CustomerId ?? Guid.Empty;
                    if (customerId == Guid.Empty)
                    {
                        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                        if (order != null) customerId = order.CustomerId;
                    }
                }
                else
                {
                    var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                    var customer = customers.FirstOrDefault();
                    if (customer == null) return BadRequest("Профиль клиента не найден");
                    customerId = customer.Id;
                    executorId = request.ExecutorId ?? Guid.Empty;
                    if (executorId == Guid.Empty)
                    {
                        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                        if (order != null && order.ExecutorId.HasValue)
                            executorId = order.ExecutorId.Value;
                    }
                }

                if (customerId == Guid.Empty || executorId == Guid.Empty)
                    return BadRequest("Не удалось определить заказчика или исполнителя");

                var createDto = new CreateDealDto
                {
                    OrderId = request.OrderId,
                    CustomerId = customerId,
                    ExecutorId = executorId,
                    ServiceId = request.ServiceId,
                    AgreedPrice = request.AgreedPrice,
                    Deadline = request.Deadline,
                    Terms = request.Terms
                };

                var deal = await _dealService.CreateAsync(createDto);

                var order2 = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                if (order2 != null && !order2.ExecutorId.HasValue)
                {
                    order2.ExecutorId = executorId;
                    await _unitOfWork.Orders.UpdateAsync(order2);
                    await _unitOfWork.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetById), new { id = deal.Id }, deal);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating deal from order");
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<ActionResult<DealDto>> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                var deal = await _dealService.ChangeStatusAsync(id, request.Status);
                return Ok(deal);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/messages")]
        [Authorize]
        public async Task<ActionResult<DealMessageDto>> AddMessage(Guid id, [FromBody] AddMessageRequest request)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Unknown";
            var userRole = User.IsInRole("Admin") ? "Администратор"
                : User.IsInRole("Manager") ? "Менеджер"
                : User.IsInRole("Executor") ? "Исполнитель"
                : "Заказчик";

            try
            {
                var message = await _dealService.AddMessageAsync(id, userName, userRole, request.Text);
                return Ok(message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DealMessageDto>>> GetMessages(Guid id)
        {
            var messages = await _dealService.GetMessagesAsync(id);
            return Ok(messages);
        }

        [HttpPost("{id}/rate-customer")]
        [Authorize]
        public async Task<ActionResult<DealDto>> RateByCustomer(Guid id, [FromBody] RateRequest request)
        {
            try
            {
                var deal = await _dealService.RateByCustomerAsync(id, request.Rating, request.Review);
                return Ok(deal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/rate-executor")]
        [Authorize]
        public async Task<ActionResult<DealDto>> RateByExecutor(Guid id, [FromBody] RateRequest request)
        {
            try
            {
                var deal = await _dealService.RateByExecutorAsync(id, request.Rating, request.Review);
                return Ok(deal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetByOrder(Guid orderId)
        {
            var deals = await _dealService.GetDealsByOrderAsync(orderId);
            return Ok(deals);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetByCustomer(Guid customerId)
        {
            var deals = await _dealService.GetDealsByCustomerAsync(customerId);
            return Ok(deals);
        }

        [HttpGet("by-executor/{executorId}")]
        public async Task<ActionResult<IEnumerable<DealDto>>> GetByExecutor(Guid executorId)
        {
            var deals = await _dealService.GetDealsByExecutorAsync(executorId);
            return Ok(deals);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _dealService.DeleteAsync(id);
            LogDeleted(id, "Сделка");
            return NoContent();
        }
    }

    public class CreateDealFromOrderRequest
    {
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ExecutorId { get; set; }
        public Guid? ServiceId { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Terms { get; set; }
    }

    public class ChangeStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class AddMessageRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class RateRequest
    {
        public int Rating { get; set; }
        public string? Review { get; set; }
    }
}
