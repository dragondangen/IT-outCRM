using System.Security.Claims;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, IUnitOfWork unitOfWork, ILogger<OrdersController> logger)
            : base(logger)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Получить все заказы
        /// </summary>
        /// <returns>Список всех заказов в системе</returns>
        /// <remarks>
        /// Возвращает полный список всех заказов со связанными данными (клиент, исполнитель, статус).
        /// </remarks>
        /// <response code="200">Список заказов успешно получен</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы с пагинацией
        /// </summary>
        /// <param name="pageNumber">Номер страницы (начинается с 1)</param>
        /// <param name="pageSize">Количество записей на странице (по умолчанию 10)</param>
        /// <returns>Пагинированный результат с заказами</returns>
        /// <remarks>
        /// Возвращает заказы с пагинацией для удобной работы с большими объемами данных.
        /// </remarks>
        /// <response code="200">Пагинированный список заказов успешно получен</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetPagedAsync(pageNumber, pageSize);
            return Ok(orders);  
        }

        /// <summary>
        /// Получить заказы текущего пользователя
        /// </summary>
        /// <returns>Список заказов текущего пользователя</returns>
        /// <remarks>
        /// Возвращает заказы в зависимости от роли пользователя:
        /// - **Admin/Manager**: все заказы в системе
        /// - **Executor**: заказы, где пользователь назначен исполнителем
        /// - **Customer/User**: только заказы, созданные текущим клиентом
        /// 
        /// Для определения клиента/исполнителя используется цепочка:
        /// User Email → ContactPerson → Company → Customer/Executor
        /// </remarks>
        /// <response code="200">Список заказов успешно получен</response>
        /// <response code="400">Ошибка: не найдено контактное лицо, компания или профиль клиента/исполнителя для текущего пользователя</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("my-orders")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRoles = string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value));
            
            Logger.LogInformation("GetMyOrders: Called for user {Email} with roles: {Roles}", userEmail ?? "unknown", userRoles);
            
            // Для админов и менеджеров показываем все заказы
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                Logger.LogInformation("GetMyOrders: User is Admin/Manager, returning all orders");
                var allOrders = await _orderService.GetAllAsync();
                Logger.LogInformation("GetMyOrders: Returned {Count} orders for Admin/Manager", allOrders.Count());
                return Ok(allOrders);
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                Logger.LogWarning("GetMyOrders: User email not found in claims");
                return BadRequest("Email пользователя не найден. Пожалуйста, войдите в систему заново.");
            }

            try
            {
                // Chain: User Email -> ContactPerson -> Company
                Logger.LogInformation("GetMyOrders: Looking up contact for email {Email}", userEmail);
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null)
                {
                    Logger.LogWarning("GetMyOrders: Contact person not found for email {Email}", userEmail);
                    return BadRequest("Контактное лицо не найдено для вашего пользователя. Обратитесь к администратору.");
                }

                Logger.LogInformation("GetMyOrders: Found contact {ContactId}, looking up company", contact.Id);
                var companies = await _unitOfWork.Companies.GetAllAsync();
                var company = companies.FirstOrDefault(c => c.ContactPersonID == contact.Id);
                
                if (company == null)
                {
                    Logger.LogWarning("GetMyOrders: Company not found for contact {ContactId}", contact.Id);
                    return BadRequest("Компания не найдена для вашего пользователя. Обратитесь к администратору.");
                }

                Logger.LogInformation("GetMyOrders: Found company {CompanyId}", company.Id);

                // Для исполнителей показываем заказы, где они назначены
                if (User.IsInRole("Executor"))
                {
                    Logger.LogInformation("GetMyOrders: User is Executor, looking up executor profile");
                    var executors = await _unitOfWork.Executors.GetAllAsync();
                    var executor = executors.FirstOrDefault(e => e.CompanyId == company.Id);
                    
                    if (executor == null)
                    {
                        Logger.LogWarning("GetMyOrders: Executor not found for company {CompanyId}", company.Id);
                        return BadRequest("Профиль исполнителя не найден для вашего пользователя. Обратитесь к администратору.");
                    }

                    Logger.LogInformation("GetMyOrders: Found executor {ExecutorId} for user {Email}", executor.Id, userEmail);
                    var executorOrders = await _orderService.GetOrdersByExecutorAsync(executor.Id);
                    Logger.LogInformation("GetMyOrders: Returned {Count} orders for executor {ExecutorId}", executorOrders.Count(), executor.Id);
                    return Ok(executorOrders);
                }

                // Для клиентов показываем только их заказы
                Logger.LogInformation("GetMyOrders: User is Customer, looking up customer profile");
                var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                var customer = customers.FirstOrDefault();
                
                if (customer == null)
                {
                    Logger.LogWarning("GetMyOrders: Customer not found for company {CompanyId}. Total customers in company: {Count}", 
                        company.Id, customers.Count());
                    return BadRequest("Профиль клиента не найден для вашего пользователя. Возможно, вы еще не создали профиль клиента. Обратитесь к администратору.");
                }

                Logger.LogInformation("GetMyOrders: Found customer {CustomerId} for user {Email}", customer.Id, userEmail);
                var customerOrders = await _orderService.GetOrdersByCustomerAsync(customer.Id);
                Logger.LogInformation("GetMyOrders: Returned {Count} orders for customer {CustomerId}", customerOrders.Count(), customer.Id);
                return Ok(customerOrders);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting my orders for user {Email}", userEmail);
                return StatusCode(500, $"Ошибка при получении заказов: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        /// <param name="id">Уникальный идентификатор заказа (GUID)</param>
        /// <returns>Данные заказа</returns>
        /// <remarks>
        /// Возвращает полную информацию о заказе, включая связанные данные (клиент, исполнитель, статус).
        /// </remarks>
        /// <response code="200">Заказ успешно найден</response>
        /// <response code="404">Заказ с указанным ID не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            Logger.LogInformation("GetById called for order ID: {OrderId}", id);
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    Logger.LogWarning("Order not found: {OrderId}", id);
                    return NotFound($"Заказ с ID {id} не найден");
                }
                
                Logger.LogInformation("Order found: {OrderId}, Name: {OrderName}", order.Id, order.Name);
                return Ok(order);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, "Ошибка при получении заказа");
            }
        }

        /// <summary>
        /// Создать новый заказ
        /// </summary>
        /// <param name="createDto">Данные для создания заказа</param>
        /// <returns>Созданный заказ</returns>
        /// <remarks>
        /// Создает новый заказ в системе. Логика работы зависит от роли пользователя:
        /// 
        /// **Для клиентов (Customer/User):**
        /// - CustomerId определяется автоматически на основе текущего пользователя
        /// - OrderStatusId устанавливается автоматически в "Опубликован"
        /// - ExecutorId и SupportTeamId не обязательны (могут быть null)
        /// 
        /// **Для администраторов и менеджеров (Admin/Manager):**
        /// - Все поля должны быть указаны вручную
        /// - CustomerId и OrderStatusId обязательны
        /// 
        /// Для определения клиента используется цепочка:
        /// User Email → ContactPerson → Company → Customer
        /// </remarks>
        /// <response code="201">Заказ успешно создан</response>
        /// <response code="400">Ошибка валидации данных или не найдены необходимые сущности</response>
        /// <response code="401">Пользователь не авторизован</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
        {
            // Если валидация уже сработала и есть ошибки для полей, которые мы заполним автоматически,
            // очистим их перед заполнением
            if (!ModelState.IsValid)
            {
                // Очищаем ошибки для полей, которые будут заполнены автоматически для клиентов
                if (User.IsInRole("User") || User.IsInRole("Customer"))
                {
                    ModelState.Remove(nameof(createDto.CustomerId));
                    ModelState.Remove(nameof(createDto.OrderStatusId));
                    ModelState.Remove(nameof(createDto.ExecutorId));
                    ModelState.Remove(nameof(createDto.SupportTeamId));
                }
                
                // Логируем ошибки валидации для отладки
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();
                
                if (validationErrors.Any())
                {
                    Logger.LogWarning("Validation errors for order creation: {Errors}", string.Join(", ", validationErrors));
                }
            }
            
            // Logic to auto-assign CustomerId and Status if user is a Customer
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            if (User.IsInRole("User") || User.IsInRole("Customer")) // Assuming "User" role is used for Customers based on Register logic
            {
                if (string.IsNullOrEmpty(userEmail))
                    return BadRequest("User email not found in claims");

                // Find CustomerId
                // Chain: User Email -> ContactPerson -> Company -> Customer
                // Since I can't easily query this via existing repositories without expanding them,
                // I'll do a direct query via generic repos if possible or standard LINQ if exposed.
                // I'll use specific repositories logic where possible.
                
                var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
                if (contact == null)
                    return BadRequest("Contact person not found for current user");

                // Find Company
                // I have to scan companies or add a method. I'll scan for now (inefficient but works for prototype)
                // Or use the fact that I need to match ContactPersonId
                // Actually, let's iterate all companies? No.
                // Let's use a raw query or add a method to CompanyRepository?
                // I'll add a specific query here using standard LINQ if I could access DbSet. 
                // But I only have Repo interface. 
                // I will use GetAllAsync and filter in memory (bad practice but quick fix) 
                // OR assume the user provides CustomerId? 
                // The prompt says "auto assign".
                
                // Better: Add GetByContactPersonIdAsync to ICompanyRepository.
                // I'll do that in parallel or after. For now, assume I can find it.
                // Wait, I have _unitOfWork.Companies. 
                // Let's use GetAllAsync() temporarily.
                var companies = await _unitOfWork.Companies.GetAllAsync();
                var company = companies.FirstOrDefault(c => c.ContactPersonID == contact.Id); // Note: ContactPersonID (case sensitive?)
                
                if (company == null)
                    return BadRequest("Company not found for current user");

                var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                var customer = customers.FirstOrDefault();
                
                if (customer == null)
                    return BadRequest("Customer profile not found for current user");

                createDto.CustomerId = customer.Id;
                
                // Set status to "Опубликован"
                var statusId = await _orderService.GetStatusIdByNameAsync("Опубликован");
                if (statusId == Guid.Empty)
                {
                    // Fallback or create?
                    // If status doesn't exist, maybe "Published"?
                    statusId = await _orderService.GetStatusIdByNameAsync("Published");
                }
                
                if (statusId == Guid.Empty)
                {
                    Logger.LogWarning("Status 'Опубликован' not found. Order creation may fail.");
                    return BadRequest("Статус 'Опубликован' не найден в системе. Обратитесь к администратору.");
                }
                
                createDto.OrderStatusId = statusId;

                // Ensure ExecutorId is null
                createDto.ExecutorId = null;
                
                // SupportTeamId необязателен для клиентов - они могут назначить команду поддержки позже
                // Просто оставляем его null, если не передан
            }
            
            // For Admins/Managers, validate that they provided required fields
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                if (!createDto.CustomerId.HasValue || createDto.CustomerId.Value == Guid.Empty)
                {
                    return BadRequest("ID клиента обязателен для администраторов и менеджеров");
                }
                
                if (!createDto.OrderStatusId.HasValue || createDto.OrderStatusId.Value == Guid.Empty)
                {
                    return BadRequest("Статус заказа обязателен для администраторов и менеджеров");
                }
                
                // SupportTeamId необязателен даже для админов/менеджеров - может быть назначен позже
            }
            
            // Validate required fields after auto-fill for all users
            if (!createDto.OrderStatusId.HasValue || createDto.OrderStatusId.Value == Guid.Empty)
            {
                return BadRequest("Статус заказа обязателен");
            }
            
            // SupportTeamId больше не является обязательным полем - может быть null
            
            if (!createDto.CustomerId.HasValue || createDto.CustomerId.Value == Guid.Empty)
            {
                return BadRequest("ID клиента обязателен");
            }
            
            // Проверяем валидность модели после заполнения полей
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();
                
                if (errors.Any())
                {
                    return BadRequest(new { errors = errors });
                }
            }

            try
            {
                var order = await _orderService.CreateAsync(createDto);
                LogCreated(order, order.Id, "Заказ");
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating order");
                return BadRequest($"Ошибка при создании заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить заказ
        /// </summary>
        /// <param name="id">Уникальный идентификатор заказа (GUID)</param>
        /// <param name="updateDto">Обновленные данные заказа</param>
        /// <returns>Обновленный заказ</returns>
        /// <remarks>
        /// Обновляет данные существующего заказа. Доступно только для администраторов и менеджеров.
        /// </remarks>
        /// <response code="200">Заказ успешно обновлен</response>
        /// <response code="400">Ошибка: ID в URL не совпадает с ID в теле запроса</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав (требуется роль Admin или Manager)</response>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderDto>> Update(Guid id, [FromBody] UpdateOrderDto updateDto)
        {
            var validationResult = ValidateUpdateId<OrderDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var order = await _orderService.UpdateAsync(updateDto);
            LogUpdated(id, "Заказ");
            return Ok(order);
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        /// <param name="id">Уникальный идентификатор заказа (GUID)</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Удаляет заказ из системы. Доступно только для администраторов.
        /// </remarks>
        /// <response code="204">Заказ успешно удален</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="403">Недостаточно прав (требуется роль Admin)</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _orderService.DeleteAsync(id);
            LogDeleted(id, "Заказ");
            return NoContent();
        }

        /// <summary>
        /// Взять заказ в работу (для Исполнителей)
        /// </summary>
        /// <param name="id">Уникальный идентификатор заказа (GUID)</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Позволяет исполнителю взять заказ в работу. 
        /// Исполнитель определяется автоматически на основе текущего авторизованного пользователя.
        /// 
        /// Для определения исполнителя используется цепочка:
        /// User Email → ContactPerson → Company → Executor
        /// </remarks>
        /// <response code="200">Заказ успешно взят в работу</response>
        /// <response code="400">Ошибка: профиль исполнителя не найден или заказ уже назначен другому исполнителю</response>
        /// <response code="401">Пользователь не авторизован</response>
        [Authorize]
        [HttpPost("{id}/take")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TakeOrder(Guid id)
        {
             var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
             if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

             // Find ExecutorId for current user
             var contact = await _unitOfWork.ContactPersons.GetByEmailAsync(userEmail);
             if (contact == null) return BadRequest("User profile not found");
             
             // Inefficient find
             var companies = await _unitOfWork.Companies.GetAllAsync();
             var company = companies.FirstOrDefault(c => c.ContactPersonID == contact.Id);
             if (company == null) return BadRequest("Company not found");

             // Find Executor
             // I need GetExecutorsByCompanyAsync? 
             // I'll scan again (bad)
             var executors = await _unitOfWork.Executors.GetAllAsync();
             var executor = executors.FirstOrDefault(e => e.CompanyId == company.Id);
             
             if (executor == null) return BadRequest("Executor profile not found. Are you an executor?");

             try 
             {
                await _orderService.TakeOrderAsync(id, executor.Id);
                return Ok();
             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
        }

        /// <summary>
        /// Получить заказы по клиенту
        /// </summary>
        /// <param name="customerId">Уникальный идентификатор клиента (GUID)</param>
        /// <returns>Список заказов указанного клиента</returns>
        /// <remarks>
        /// Возвращает все заказы, созданные указанным клиентом.
        /// </remarks>
        /// <response code="200">Список заказов клиента успешно получен</response>
        [HttpGet("by-customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomer(Guid customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы по исполнителю
        /// </summary>
        /// <param name="executorId">Уникальный идентификатор исполнителя (GUID)</param>
        /// <returns>Список заказов, назначенных указанному исполнителю</returns>
        /// <remarks>
        /// Возвращает все заказы, где указанный исполнитель назначен в качестве ответственного.
        /// </remarks>
        /// <response code="200">Список заказов исполнителя успешно получен</response>
        [HttpGet("by-executor/{executorId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByExecutor(Guid executorId)
        {
            var orders = await _orderService.GetOrdersByExecutorAsync(executorId);
            return Ok(orders);
        }

        /// <summary>
        /// Получить заказы по статусу
        /// </summary>
        /// <param name="statusId">Уникальный идентификатор статуса заказа (GUID)</param>
        /// <returns>Список заказов с указанным статусом</returns>
        /// <remarks>
        /// Возвращает все заказы, имеющие указанный статус.
        /// </remarks>
        /// <response code="200">Список заказов по статусу успешно получен</response>
        [HttpGet("by-status/{statusId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(Guid statusId)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(statusId);
            return Ok(orders);
        }
    }
}
