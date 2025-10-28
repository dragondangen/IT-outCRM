using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все заказы
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetPagedAsync(pageNumber, pageSize);
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Заказ с ID {id} не найден");

            return Ok(order);
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
        {
            var order = await _orderService.CreateAsync(createDto);
            _logger.LogInformation("Создан заказ: {OrderName} (ID: {OrderId})", order.Name, order.Id);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        /// <summary>
        /// Обновить заказ
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] UpdateOrderDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID в URL не совпадает с ID в теле запроса");

            var order = await _orderService.UpdateAsync(updateDto);
            _logger.LogInformation("Обновлён заказ: {OrderId}", id);
            return Ok(order);
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _orderService.DeleteAsync(id);
            _logger.LogInformation("Удалён заказ: {OrderId}", id);
            return NoContent();
        }

        /// <summary>
        /// Получить заказы по клиенту
        /// </summary>
        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы по исполнителю
        /// </summary>
        [HttpGet("by-executor/{executorId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByExecutor(Guid executorId)
        {
            var orders = await _orderService.GetOrdersByExecutorAsync(executorId);
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы по статусу
        /// </summary>
        [HttpGet("by-status/{statusId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(Guid statusId)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(statusId);
            return Ok(orders);
        }
    }
}

