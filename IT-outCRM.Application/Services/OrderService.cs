using AutoMapper;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
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
            return order != null ? _mapper.Map<OrderDto>(order) : null;
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
            await _validationService.EnsureCustomerExistsAsync(createDto.CustomerId);
            await _validationService.EnsureExecutorExistsAsync(createDto.ExecutorId);
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
    }
}

