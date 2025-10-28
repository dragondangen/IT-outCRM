using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IContactPersonRepository : IGenericRepository<ContactPerson>
    {
        Task<ContactPerson?> GetByEmailAsync(string email);
        Task<ContactPerson?> GetByPhoneAsync(string phoneNumber);
    }
}

