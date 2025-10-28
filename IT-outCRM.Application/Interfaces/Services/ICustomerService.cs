using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Customer;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<CustomerDto> CreateAsync(CreateCustomerDto createDto);
        Task<CustomerDto> UpdateAsync(UpdateCustomerDto updateDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<CustomerDto>> GetCustomersByCompanyAsync(Guid companyId);
    }
}

