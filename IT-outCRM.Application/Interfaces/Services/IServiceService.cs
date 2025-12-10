using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Service;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync();
        Task<PagedResult<ServiceDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ServiceDto?> GetByIdAsync(Guid id);
        Task<ServiceDto> CreateAsync(CreateServiceDto createDto);
        Task<ServiceDto> UpdateAsync(UpdateServiceDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ServiceDto>> GetServicesByExecutorAsync(Guid executorId);
        Task<IEnumerable<ServiceDto>> GetActiveServicesAsync();
    }
}

