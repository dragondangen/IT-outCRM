using IT_outCRM.Application.DTOs.Deal;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IDealService : IBaseService<DealDto, CreateDealDto, UpdateDealDto>
    {
        Task<IEnumerable<DealDto>> GetDealsByCustomerAsync(Guid customerId);
        Task<IEnumerable<DealDto>> GetDealsByExecutorAsync(Guid executorId);
        Task<IEnumerable<DealDto>> GetDealsByOrderAsync(Guid orderId);
        Task<IEnumerable<DealDto>> GetDealsByStatusAsync(string status);
        Task<DealDto?> GetDealWithDetailsAsync(Guid id);
        Task<DealDto> ChangeStatusAsync(Guid dealId, string newStatus);
        Task<DealDto> RateByCustomerAsync(Guid dealId, int rating, string? review);
        Task<DealDto> RateByExecutorAsync(Guid dealId, int rating, string? review);
        Task<DealMessageDto> AddMessageAsync(Guid dealId, string senderName, string senderRole, string text);
        Task<IEnumerable<DealMessageDto>> GetMessagesAsync(Guid dealId);
    }
}
