using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetCompanyByInnAsync(string inn)
        {
            return await _dbSet
                .Include(c => c.ContactPerson)
                .FirstOrDefaultAsync(c => c.Inn == inn);
        }

        public async Task<Company?> GetCompanyWithContactPersonAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.ContactPerson)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

