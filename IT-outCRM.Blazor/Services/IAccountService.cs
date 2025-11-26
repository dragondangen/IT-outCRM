using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IAccountService
    {
        Task<List<AccountModel>> GetAllAsync();
        Task<AccountModel?> GetByIdAsync(Guid id);
        Task<AccountModel?> CreateAsync(AccountModel model);
        Task<AccountModel?> UpdateAsync(AccountModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}
