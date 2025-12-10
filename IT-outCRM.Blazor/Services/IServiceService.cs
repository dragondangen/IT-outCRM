using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IServiceService
    {
        Task<List<ServiceModel>> GetAllAsync();
        Task<PagedResult<ServiceModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceModel?> GetByIdAsync(Guid id);
        Task<ServiceModel?> CreateAsync(CreateServiceModel model);
        Task<ServiceModel?> UpdateAsync(ServiceModel model);
        Task<bool> DeleteAsync(Guid id);
        Task<List<ServiceModel>> GetServicesByExecutorAsync(Guid executorId);
        Task<List<ServiceModel>> GetMyServicesAsync();
        void SetToken(string token);
    }
}

