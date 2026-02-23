using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IDealRepository : IGenericRepository<Deal>
    {
        Task<IEnumerable<Deal>> GetDealsByCustomerAsync(Guid customerId);
        Task<IEnumerable<Deal>> GetDealsByExecutorAsync(Guid executorId);
        Task<IEnumerable<Deal>> GetDealsByOrderAsync(Guid orderId);
        Task<IEnumerable<Deal>> GetDealsByStatusAsync(string status);
        Task<Deal?> GetDealWithDetailsAsync(Guid id);
    }
}
