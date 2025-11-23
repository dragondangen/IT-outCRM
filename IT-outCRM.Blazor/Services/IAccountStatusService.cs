using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IAccountStatusService
    {
        Task<List<AccountStatusModel>> GetAllAsync();
    }
}



