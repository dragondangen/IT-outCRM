using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Executor;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IExecutorService
    {
        Task<ExecutorDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ExecutorDto>> GetAllAsync();
        Task<PagedResult<ExecutorDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ExecutorDto> CreateAsync(CreateExecutorDto createDto);
        Task<ExecutorDto> UpdateAsync(UpdateExecutorDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ExecutorDto>> GetTopExecutorsAsync(int count);
    }
}

