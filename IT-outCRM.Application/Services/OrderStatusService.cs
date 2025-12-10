using AutoMapper;
using IT_outCRM.Application.DTOs.OrderStatus;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class OrderStatusService : BaseService<OrderStatus, OrderStatusDto, CreateOrderStatusDto, UpdateOrderStatusDto>, IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepository;

        public OrderStatusService(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            _orderStatusRepository = unitOfWork.OrderStatuses;
        }

        protected override IGenericRepository<OrderStatus> Repository => _orderStatusRepository;

        protected override async Task ValidateCreateAsync(CreateOrderStatusDto createDto)
        {
            // Проверка уникальности названия
            if (await _orderStatusRepository.NameExistsAsync(createDto.Name))
            {
                throw new InvalidOperationException($"Статус заказа с названием '{createDto.Name}' уже существует");
            }
        }

        protected override async Task ValidateUpdateAsync(UpdateOrderStatusDto updateDto, OrderStatus existingEntity)
        {
            // Проверка уникальности названия (исключая текущую запись)
            if (await _orderStatusRepository.NameExistsAsync(updateDto.Name, updateDto.Id))
            {
                throw new InvalidOperationException($"Статус заказа с названием '{updateDto.Name}' уже существует");
            }
        }
    }
}








