using IT_outCRM.Application.DTOs.AccountStatus;
using IT_outCRM.Application.DTOs.Common;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IAccountStatusService
    {
        Task<IEnumerable<AccountStatusDto>> GetAllAsync();
        Task<PagedResult<AccountStatusDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<AccountStatusDto?> GetByIdAsync(Guid id);
        Task<AccountStatusDto> CreateAsync(CreateAccountStatusDto createDto);
        Task<AccountStatusDto> UpdateAsync(UpdateAccountStatusDto updateDto);
        Task DeleteAsync(Guid id);
    }
}

