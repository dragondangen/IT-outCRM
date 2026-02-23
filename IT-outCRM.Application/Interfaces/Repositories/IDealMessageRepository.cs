using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IDealMessageRepository : IGenericRepository<DealMessage>
    {
        Task<IEnumerable<DealMessage>> GetMessagesByDealAsync(Guid dealId);
    }
}
