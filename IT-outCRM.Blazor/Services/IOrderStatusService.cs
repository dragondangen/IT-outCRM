using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IOrderStatusService
    {
        Task<List<OrderStatusModel>> GetAllAsync();
    }
}

