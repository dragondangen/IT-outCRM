using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class DealRepository : GenericRepository<Deal>, IDealRepository
    {
        public DealRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Deal>> GetDealsByCustomerAsync(Guid customerId)
        {
            return await _dbSet
                .Include(d => d.Order).ThenInclude(o => o!.OrderStatus)
                .Include(d => d.Customer).ThenInclude(c => c!.Account)
                .Include(d => d.Executor).ThenInclude(e => e!.Account)
                .Include(d => d.Service)
                .Where(d => d.CustomerId == customerId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Deal>> GetDealsByExecutorAsync(Guid executorId)
        {
            return await _dbSet
                .Include(d => d.Order).ThenInclude(o => o!.OrderStatus)
                .Include(d => d.Customer).ThenInclude(c => c!.Account)
                .Include(d => d.Executor).ThenInclude(e => e!.Account)
                .Include(d => d.Service)
                .Where(d => d.ExecutorId == executorId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Deal>> GetDealsByOrderAsync(Guid orderId)
        {
            return await _dbSet
                .Include(d => d.Customer).ThenInclude(c => c!.Account)
                .Include(d => d.Executor).ThenInclude(e => e!.Account)
                .Include(d => d.Service)
                .Where(d => d.OrderId == orderId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Deal>> GetDealsByStatusAsync(string status)
        {
            return await _dbSet
                .Include(d => d.Order).ThenInclude(o => o!.OrderStatus)
                .Include(d => d.Customer).ThenInclude(c => c!.Account)
                .Include(d => d.Executor).ThenInclude(e => e!.Account)
                .Include(d => d.Service)
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Deal?> GetDealWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.Order).ThenInclude(o => o!.OrderStatus)
                .Include(d => d.Customer).ThenInclude(c => c!.Account)
                .Include(d => d.Customer).ThenInclude(c => c!.Company)
                .Include(d => d.Executor).ThenInclude(e => e!.Account)
                .Include(d => d.Executor).ThenInclude(e => e!.Company)
                .Include(d => d.Service)
                .Include(d => d.Messages!.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
