using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IDealService
    {
        Task<List<DealModel>> GetAllAsync();
        Task<DealModel?> GetByIdAsync(Guid id);
        Task<List<DealModel>> GetMyDealsAsync();
        Task<DealModel?> CreateFromOrderAsync(CreateDealFromOrderModel model);
        Task<DealModel?> ChangeStatusAsync(Guid dealId, string newStatus);
        Task<List<DealMessageModel>> GetMessagesAsync(Guid dealId);
        Task<DealMessageModel?> AddMessageAsync(Guid dealId, string text);
        Task<DealModel?> RateByCustomerAsync(Guid dealId, int rating, string? review);
        Task<DealModel?> RateByExecutorAsync(Guid dealId, int rating, string? review);
        Task<List<DealModel>> GetByOrderAsync(Guid orderId);
        Task DeleteAsync(Guid id);
        void SetToken(string token);
    }
}
