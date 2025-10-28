using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Company)
                    .ThenInclude(comp => comp!.ContactPerson)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetCustomersByCompanyAsync(Guid companyId)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Company)
                .Where(c => c.CompanyId == companyId)
                .ToListAsync();
        }
    }
}

