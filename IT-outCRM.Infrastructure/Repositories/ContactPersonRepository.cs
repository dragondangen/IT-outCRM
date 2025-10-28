using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Infrastructure.Repositories
{
    public class ContactPersonRepository : GenericRepository<ContactPerson>, IContactPersonRepository
    {
        public ContactPersonRepository(CrmDbContext context) : base(context)
        {
        }

        public async Task<ContactPerson?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(cp => cp.Email == email);
        }

        public async Task<ContactPerson?> GetByPhoneAsync(string phoneNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(cp => cp.PhoneNumber == phoneNumber);
        }
    }
}

