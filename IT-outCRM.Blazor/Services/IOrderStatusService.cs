using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IOrderStatusService
    {
        Task<List<OrderStatusModel>> GetAllAsync();
        Task<OrderStatusModel?> GetByIdAsync(Guid id);
        Task<OrderStatusModel?> CreateAsync(OrderStatusModel model);
        Task<OrderStatusModel?> UpdateAsync(OrderStatusModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}


