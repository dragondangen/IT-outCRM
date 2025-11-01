using AutoMapper;
using IT_outCRM.Application.DTOs.ContactPerson;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с контактными лицами
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class ContactPersonService : BaseService<ContactPerson, ContactPersonDto, CreateContactPersonDto, UpdateContactPersonDto>, IContactPersonService
    {
        private readonly IContactPersonRepository _contactPersonRepository;

        public ContactPersonService(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            _contactPersonRepository = unitOfWork.ContactPersons;
        }

        protected override IGenericRepository<ContactPerson> Repository => _contactPersonRepository;

        /// <summary>
        /// Валидация при создании - проверка на дубликат email
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateContactPersonDto createDto)
        {
            var existingPerson = await _contactPersonRepository.GetByEmailAsync(createDto.Email);
            if (existingPerson != null)
                throw new InvalidOperationException($"Contact person with email {createDto.Email} already exists");
        }

        /// <summary>
        /// Валидация при обновлении - проверка на дубликат email (если email изменен)
        /// </summary>
        protected override async Task ValidateUpdateAsync(UpdateContactPersonDto updateDto, ContactPerson existingPerson)
        {
            if (existingPerson.Email != updateDto.Email)
            {
                var personWithSameEmail = await _contactPersonRepository.GetByEmailAsync(updateDto.Email);
                if (personWithSameEmail != null)
                    throw new InvalidOperationException($"Contact person with email {updateDto.Email} already exists");
            }
        }

        /// <summary>
        /// Получить контактное лицо по email (специфичный метод для ContactPerson)
        /// </summary>
        public async Task<ContactPersonDto?> GetByEmailAsync(string email)
        {
            var contactPerson = await _contactPersonRepository.GetByEmailAsync(email);
            return contactPerson != null ? _mapper.Map<ContactPersonDto>(contactPerson) : null;
        }
    }
}

