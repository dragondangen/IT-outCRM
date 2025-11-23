using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IAccountService
    {
        Task<List<AccountModel>> GetAllAsync();
        Task<bool> DeleteAsync(Guid id);
    }
}



