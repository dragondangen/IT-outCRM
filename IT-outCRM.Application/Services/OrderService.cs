using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(id);
            return order != null ? _mapper.Map<OrderDto>(order) : null;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            var totalCount = await _unitOfWork.Orders.CountAsync();

            var pagedOrders = orders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<OrderDto>
            {
                Items = _mapper.Map<List<OrderDto>>(pagedOrders),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto createDto)
        {
            // Проверка существования связанных сущностей
            if (!await _unitOfWork.Customers.ExistsAsync(createDto.CustomerId))
                throw new KeyNotFoundException($"Customer with ID {createDto.CustomerId} not found");

            if (!await _unitOfWork.Executors.ExistsAsync(createDto.ExecutorId))
                throw new KeyNotFoundException($"Executor with ID {createDto.ExecutorId} not found");

            var order = _mapper.Map<Order>(createDto);
            order.Id = Guid.NewGuid();

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(order.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created order");
        }

        public async Task<OrderDto> UpdateAsync(UpdateOrderDto updateDto)
        {
            var existingOrder = await _unitOfWork.Orders.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Order with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingOrder);

            await _unitOfWork.Orders.UpdateAsync(existingOrder);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingOrder.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated order");
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Order with ID {id} not found");

            await _unitOfWork.Orders.DeleteAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(Guid customerId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerAsync(customerId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByExecutorAsync(Guid executorId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByExecutorAsync(executorId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(Guid statusId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}

