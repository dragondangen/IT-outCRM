using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId);
        Task<IEnumerable<Order>> GetOrdersByExecutorAsync(Guid executorId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(Guid statusId);
        Task<Order?> GetOrderWithDetailsAsync(Guid id);
    }
}

