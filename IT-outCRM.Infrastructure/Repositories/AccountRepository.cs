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
    }
}

