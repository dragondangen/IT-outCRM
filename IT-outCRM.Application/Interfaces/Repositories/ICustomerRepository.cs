using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetCustomerWithDetailsAsync(Guid id);
        Task<IEnumerable<Customer>> GetCustomersByCompanyAsync(Guid companyId);
    }
}

