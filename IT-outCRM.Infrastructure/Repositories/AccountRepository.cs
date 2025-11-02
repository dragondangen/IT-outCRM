using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(Guid statusId)
        {
            return await _dbSet
                .Include(a => a.AccountStatus)
                .Where(a => a.AccountStatusId == statusId)
                .ToListAsync();
        }

        public async Task<Account?> GetAccountWithStatusAsync(Guid id)
        {
            return await _dbSet
                .Include(a => a.AccountStatus)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetAllWithStatusAsync()
        {
            return await _dbSet
                .Include(a => a.AccountStatus)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Account> items, int totalCount)> GetPagedWithStatusAsync(int pageNumber, int pageSize)
        {
            var query = _dbSet.Include(a => a.AccountStatus);
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}

