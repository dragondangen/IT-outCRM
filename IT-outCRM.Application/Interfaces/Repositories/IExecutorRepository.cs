using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IExecutorRepository : IGenericRepository<Executor>
    {
        Task<Executor?> GetExecutorWithDetailsAsync(Guid id);
        Task<IEnumerable<Executor>> GetTopExecutorsAsync(int count);
        Task<Executor?> GetByCompanyIdAsync(Guid companyId);
    }
}

