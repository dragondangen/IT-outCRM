using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetCompanyByInnAsync(string inn);
        Task<Company?> GetCompanyWithContactPersonAsync(Guid id);
        Task<Company?> GetByContactPersonIdAsync(Guid contactPersonId);
    }
}

