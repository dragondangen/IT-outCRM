using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class DealMessageRepository : GenericRepository<DealMessage>, IDealMessageRepository
    {
        public DealMessageRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DealMessage>> GetMessagesByDealAsync(Guid dealId)
        {
            return await _dbSet
                .Where(m => m.DealId == dealId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
