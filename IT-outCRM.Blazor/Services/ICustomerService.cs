using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerModel>> GetAllAsync();
        Task<PagedResult<CustomerModel>> GetPagedAsync(int pageNumber = 1, int pageSize = 10);
        Task<CustomerModel?> GetByIdAsync(Guid id);
        Task<CustomerModel?> CreateAsync(CreateCustomerModel model);
        Task<CustomerModel?> UpdateAsync(CustomerModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}

