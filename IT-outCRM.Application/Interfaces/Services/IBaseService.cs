using IT_outCRM.Application.DTOs.Common;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IBaseService<TDto, TCreateDto, TUpdateDto>
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<PagedResult<TDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<TDto> CreateAsync(TCreateDto createDto);
        Task<TDto> UpdateAsync(TUpdateDto updateDto);
        Task DeleteAsync(Guid id);
    }
}








