using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Order;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<PagedResult<OrderDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<OrderDto> CreateAsync(CreateOrderDto createDto);
        Task<OrderDto> UpdateAsync(UpdateOrderDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(Guid customerId);
        Task<IEnumerable<OrderDto>> GetOrdersByExecutorAsync(Guid executorId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(Guid statusId);
    }
}

