using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.ContactPerson;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactPersonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ContactPersonDto?> GetByIdAsync(Guid id)
        {
            var contactPerson = await _unitOfWork.ContactPersons.GetByIdAsync(id);
            return contactPerson != null ? _mapper.Map<ContactPersonDto>(contactPerson) : null;
        }

        public async Task<IEnumerable<ContactPersonDto>> GetAllAsync()
        {
            var contactPersons = await _unitOfWork.ContactPersons.GetAllAsync();
            return _mapper.Map<IEnumerable<ContactPersonDto>>(contactPersons);
        }

        public async Task<PagedResult<ContactPersonDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var contactPersons = await _unitOfWork.ContactPersons.GetAllAsync();
            var totalCount = await _unitOfWork.ContactPersons.CountAsync();

            var pagedContactPersons = contactPersons
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ContactPersonDto>
            {
                Items = _mapper.Map<List<ContactPersonDto>>(pagedContactPersons),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ContactPersonDto> CreateAsync(CreateContactPersonDto createDto)
        {
            // Проверка на дубликат email
            var existingPerson = await _unitOfWork.ContactPersons.GetByEmailAsync(createDto.Email);
            if (existingPerson != null)
                throw new InvalidOperationException($"Contact person with email {createDto.Email} already exists");

            var contactPerson = _mapper.Map<ContactPerson>(createDto);
            contactPerson.Id = Guid.NewGuid();

            await _unitOfWork.ContactPersons.AddAsync(contactPerson);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(contactPerson.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created contact person");
        }

        public async Task<ContactPersonDto> UpdateAsync(UpdateContactPersonDto updateDto)
        {
            var existingPerson = await _unitOfWork.ContactPersons.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Contact person with ID {updateDto.Id} not found");

            // Проверка на дубликат email (если email изменен)
            if (existingPerson.Email != updateDto.Email)
            {
                var personWithSameEmail = await _unitOfWork.ContactPersons.GetByEmailAsync(updateDto.Email);
                if (personWithSameEmail != null)
                    throw new InvalidOperationException($"Contact person with email {updateDto.Email} already exists");
            }

            _mapper.Map(updateDto, existingPerson);

            await _unitOfWork.ContactPersons.UpdateAsync(existingPerson);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingPerson.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated contact person");
        }

        public async Task DeleteAsync(Guid id)
        {
            var contactPerson = await _unitOfWork.ContactPersons.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Contact person with ID {id} not found");

            await _unitOfWork.ContactPersons.DeleteAsync(contactPerson);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ContactPersonDto?> GetByEmailAsync(string email)
        {
            var contactPerson = await _unitOfWork.ContactPersons.GetByEmailAsync(email);
            return contactPerson != null ? _mapper.Map<ContactPersonDto>(contactPerson) : null;
        }
    }
}

