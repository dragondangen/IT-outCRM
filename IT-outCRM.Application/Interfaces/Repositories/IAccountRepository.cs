using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountsByStatusAsync(Guid statusId);
        Task<Account?> GetAccountWithStatusAsync(Guid id);
    }
}

