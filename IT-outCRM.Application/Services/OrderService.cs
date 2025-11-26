using AutoMapper;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с заказами
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class OrderService : BaseService<Order, OrderDto, CreateOrderDto, UpdateOrderDto>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEntityValidationService _validationService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IEntityValidationService validationService) 
            : base(unitOfWork, mapper)
        {
            _orderRepository = unitOfWork.Orders;
            _validationService = validationService;
        }

        protected override IGenericRepository<Order> Repository => _orderRepository;

        /// <summary>
        /// Переопределяем для загрузки связанных сущностей
        /// </summary>
        public override async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
                return null;
            
            try
            {
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                // Логируем ошибку маппинга
                throw new InvalidOperationException($"Ошибка маппинга заказа {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<OrderDto?> GetByIdAfterCreateAsync(Order entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<OrderDto?>(null);
        }

        protected override Task<OrderDto?> GetByIdAfterUpdateAsync(Order entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<OrderDto?>(null);
        }

        /// <summary>
        /// Валидация при создании - проверка существования связанных сущностей
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateOrderDto createDto)
        {
            // CustomerId должен быть заполнен контроллером к этому моменту
            if (!createDto.CustomerId.HasValue || createDto.CustomerId.Value == Guid.Empty)
            {
                throw new ArgumentException("CustomerId is required");
            }
            
            await _validationService.EnsureCustomerExistsAsync(createDto.CustomerId.Value);
            
            if (createDto.ExecutorId.HasValue)
            {
                await _validationService.EnsureExecutorExistsAsync(createDto.ExecutorId.Value);
            }
            
            // OrderStatusId должен быть заполнен контроллером к этому моменту
            if (!createDto.OrderStatusId.HasValue || createDto.OrderStatusId.Value == Guid.Empty)
            {
                throw new ArgumentException("OrderStatusId is required");
            }
            
            // SupportTeamId теперь необязателен - может быть null
        }

        /// <summary>
        /// Получить заказы по клиенту (специфичный метод для Order)
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(Guid customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        /// <summary>
        /// Получить заказы по исполнителю (специфичный метод для Order)
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetOrdersByExecutorAsync(Guid executorId)
        {
            var orders = await _orderRepository.GetOrdersByExecutorAsync(executorId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        /// <summary>
        /// Получить заказы по статусу (специфичный метод для Order)
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(Guid statusId)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task TakeOrderAsync(Guid orderId, Guid executorId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found");

            if (order.ExecutorId.HasValue)
                throw new InvalidOperationException("Order is already taken by another executor");

            // Assign executor
            order.ExecutorId = executorId;

            // Change status to "Under Review" (На рассмотрении)
            var status = await _unitOfWork.OrderStatuses.GetByNameAsync("На рассмотрении");
            
            // If status not found, maybe try English "Under Review" or default?
            // Assuming seed data exists or we rely on accurate naming.
            if (status != null)
            {
                order.OrderStatusId = status.Id;
            }

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Guid> GetStatusIdByNameAsync(string statusName)
        {
            var status = await _unitOfWork.OrderStatuses.GetByNameAsync(statusName);
            return status?.Id ?? Guid.Empty;
        }

        public async Task<Guid> GetDefaultSupportTeamIdAsync()
        {
            // Get first available SupportTeam from existing orders
            var orders = await _orderRepository.GetAllAsync();
            var firstOrder = orders.FirstOrDefault(o => o.SupportTeamId.HasValue);
            if (firstOrder != null && firstOrder.SupportTeamId.HasValue)
            {
                return firstOrder.SupportTeamId.Value;
            }
            
            // If no orders exist, try to get first SupportTeam from DB directly
            // We need to access DbContext, but we don't have direct access
            // For now, return empty and let controller handle it
            return Guid.Empty;
        }
    }
}
