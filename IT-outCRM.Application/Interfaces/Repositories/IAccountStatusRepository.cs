using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IAccountStatusRepository : IGenericRepository<AccountStatus>
    {
        Task<bool> NameExistsAsync(string name);
        Task<bool> NameExistsAsync(string name, Guid excludeId);
    }
}

