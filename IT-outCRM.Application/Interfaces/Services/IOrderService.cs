using IT_outCRM.Application.DTOs.Order;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IOrderService : IBaseService<OrderDto, CreateOrderDto, UpdateOrderDto>
    {
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(Guid customerId);
        Task<IEnumerable<OrderDto>> GetOrdersByExecutorAsync(Guid executorId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(Guid statusId);
        Task TakeOrderAsync(Guid orderId, Guid executorId);
        Task<Guid> GetStatusIdByNameAsync(string statusName);
        Task<Guid> GetDefaultSupportTeamIdAsync();
    }
}
