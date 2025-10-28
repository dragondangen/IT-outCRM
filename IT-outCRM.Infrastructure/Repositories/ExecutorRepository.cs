using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class ExecutorRepository : GenericRepository<Executor>, IExecutorRepository
    {
        public ExecutorRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<Executor?> GetExecutorWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(e => e.Account)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Executor>> GetTopExecutorsAsync(int count)
        {
            return await _dbSet
                .Include(e => e.Account)
                .Include(e => e.Company)
                .OrderByDescending(e => e.CompletedOrders)
                .Take(count)
                .ToListAsync();
        }
    }
}

