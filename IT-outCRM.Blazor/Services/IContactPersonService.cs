using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IContactPersonService
    {
        Task<List<ContactPersonModel>> GetAllAsync();
        Task<ContactPersonModel?> GetByIdAsync(Guid id);
        Task<ContactPersonModel?> CreateAsync(ContactPersonModel model);
        Task<ContactPersonModel?> UpdateAsync(ContactPersonModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}

