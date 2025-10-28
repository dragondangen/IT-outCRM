using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Company;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<PagedResult<CompanyDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<CompanyDto> CreateAsync(CreateCompanyDto createDto);
        Task<CompanyDto> UpdateAsync(UpdateCompanyDto updateDto);
        Task DeleteAsync(Guid id);
        Task<CompanyDto?> GetCompanyByInnAsync(string inn);
    }
}

