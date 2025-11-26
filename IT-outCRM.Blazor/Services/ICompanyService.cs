using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyModel>> GetAllAsync();
        Task<CompanyModel?> GetByIdAsync(Guid id);
        Task<CompanyModel?> CreateAsync(CompanyModel model);
        Task<CompanyModel?> UpdateAsync(CompanyModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}
