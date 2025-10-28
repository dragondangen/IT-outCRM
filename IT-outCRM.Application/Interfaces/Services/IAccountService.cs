using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AccountDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<AccountDto>> GetAllAsync();
        Task<PagedResult<AccountDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<AccountDto> CreateAsync(CreateAccountDto createDto);
        Task<AccountDto> UpdateAsync(UpdateAccountDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<AccountDto>> GetAccountsByStatusAsync(Guid statusId);
    }
}

