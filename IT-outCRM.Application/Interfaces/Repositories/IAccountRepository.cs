using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountsByStatusAsync(Guid statusId);
        Task<Account?> GetAccountWithStatusAsync(Guid id);
        Task<IEnumerable<Account>> GetAllWithStatusAsync();
        Task<(IEnumerable<Account> items, int totalCount)> GetPagedWithStatusAsync(int pageNumber, int pageSize);
    }
}

