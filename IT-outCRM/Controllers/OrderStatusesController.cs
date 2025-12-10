using IT_outCRM.Application.DTOs.OrderStatus;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    /// <summary>
    /// Контроллер для управления статусами заказов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class OrderStatusesController : BaseController
    {
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusesController(IOrderStatusService orderStatusService, ILogger<OrderStatusesController> logger)
            : base(logger)
        {
            _orderStatusService = orderStatusService;
        }

        /// <summary>
        /// Получить список всех статусов заказов
        /// </summary>
        /// <returns>Список всех статусов заказов</returns>
        /// <remarks>
        /// Доступно для всех авторизованных пользователей
        /// </remarks>
        /// <response code="200">Список статусов успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<OrderStatusDto>>> GetAll()
        {
            var statuses = await _orderStatusService.GetAllAsync();
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
        [ProducesResponseType(typeof(PagedResult<OrderStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<OrderStatusDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var statuses = await _orderStatusService.GetPagedAsync(pageNumber, pageSize);
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
        [ProducesResponseType(typeof(OrderStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderStatusDto>> GetById(Guid id)
        {
            var status = await _orderStatusService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<OrderStatusDto>(status, id, "Статус заказа");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(status);
        }

        /// <summary>
        /// Создать новый статус заказа
        /// </summary>
        /// <param name="createDto">Данные для создания статуса</param>
        /// <returns>Созданный статус</returns>
        /// <remarks>
        /// Доступно только для ролей: Admin, Manager
        /// 
        /// Пример запроса:
        /// 
        ///     POST /api/orderstatuses
        ///     {
        ///        "name": "Опубликован"
        ///     }
        /// </remarks>
        /// <response code="201">Статус успешно создан</response>
        /// <response code="400">Ошибка валидации данных или статус с таким названием уже существует</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав доступа</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ProducesResponseType(typeof(OrderStatusDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderStatusDto>> Create([FromBody] CreateOrderStatusDto createDto)
        {
            var status = await _orderStatusService.CreateAsync(createDto);
            LogCreated(status, status.Id, "Статус заказа");
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
        [ProducesResponseType(typeof(OrderStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderStatusDto>> Update(Guid id, [FromBody] UpdateOrderStatusDto updateDto)
        {
            var validationResult = ValidateUpdateId<OrderStatusDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var status = await _orderStatusService.UpdateAsync(updateDto);
            LogUpdated(id, "Статус заказа");
            return Ok(status);
        }

        /// <summary>
        /// Удалить статус по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса для удаления</param>
        /// <remarks>
        /// Доступно только для роли Admin
        /// 
        /// ⚠️ Внимание: Операция необратима! Убедитесь, что статус не используется в заказах.
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
            await _orderStatusService.DeleteAsync(id);
            LogDeleted(id, "Статус заказа");
            return NoContent();
        }
    }
}








