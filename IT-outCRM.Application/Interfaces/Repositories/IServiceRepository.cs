using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Repositories
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<Service?> GetServiceWithDetailsAsync(Guid id);
        Task<IEnumerable<Service>> GetServicesByExecutorAsync(Guid executorId);
        Task<IEnumerable<Service>> GetActiveServicesAsync();
    }
}

