using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.ContactPerson;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IContactPersonService
    {
        Task<ContactPersonDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ContactPersonDto>> GetAllAsync();
        Task<PagedResult<ContactPersonDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ContactPersonDto> CreateAsync(CreateContactPersonDto createDto);
        Task<ContactPersonDto> UpdateAsync(UpdateContactPersonDto updateDto);
        Task DeleteAsync(Guid id);
        Task<ContactPersonDto?> GetByEmailAsync(string email);
    }
}

