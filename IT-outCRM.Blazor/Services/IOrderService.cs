using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetAllAsync();
        Task<PagedResult<OrderModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
        Task<OrderModel?> GetByIdAsync(Guid id);
        Task<OrderModel?> CreateAsync(CreateOrderModel model);
        Task<OrderModel?> UpdateAsync(OrderModel model);
        Task<bool> DeleteAsync(Guid id);
        Task<List<OrderModel>> GetByCustomerAsync(Guid customerId);
        Task<List<OrderModel>> GetMyOrdersAsync();
        Task<bool> TakeOrderAsync(Guid id);
        void SetToken(string token);
    }
}
