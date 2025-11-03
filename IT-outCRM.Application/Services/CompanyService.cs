using AutoMapper;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с компаниями
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class CompanyService : BaseService<Company, CompanyDto, CreateCompanyDto, UpdateCompanyDto>, ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            _companyRepository = unitOfWork.Companies;
        }

        protected override IGenericRepository<Company> Repository => _companyRepository;

        /// <summary>
        /// Переопределяем для загрузки связанной сущности ContactPerson
        /// </summary>
        public override async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var company = await _companyRepository.GetCompanyWithContactPersonAsync(id);
            return company != null ? _mapper.Map<CompanyDto>(company) : null;
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<CompanyDto?> GetByIdAfterCreateAsync(Company entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<CompanyDto?>(null);
        }

        protected override Task<CompanyDto?> GetByIdAfterUpdateAsync(Company entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<CompanyDto?>(null);
        }

        /// <summary>
        /// Валидация при создании - проверка на дубликат ИНН и существование контактного лица
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateCompanyDto createDto)
        {
            var existingCompany = await _companyRepository.GetCompanyByInnAsync(createDto.Inn);
            if (existingCompany != null)
                throw new InvalidOperationException($"Company with INN {createDto.Inn} already exists");
            
            // Проверяем, что указанное контактное лицо существует
            var contactPersonExists = await _unitOfWork.ContactPersons.ExistsAsync(createDto.ContactPersonId);
            if (!contactPersonExists)
                throw new KeyNotFoundException($"Contact person with ID {createDto.ContactPersonId} not found");
        }

        /// <summary>
        /// Валидация при обновлении - проверка на дубликат ИНН и существование контактного лица
        /// </summary>
        protected override async Task ValidateUpdateAsync(UpdateCompanyDto updateDto, Company existingCompany)
        {
            if (existingCompany.Inn != updateDto.Inn)
            {
                var companyWithSameInn = await _companyRepository.GetCompanyByInnAsync(updateDto.Inn);
                if (companyWithSameInn != null)
                    throw new InvalidOperationException($"Company with INN {updateDto.Inn} already exists");
            }
            
            // Проверяем, что указанное контактное лицо существует
            var contactPersonExists = await _unitOfWork.ContactPersons.ExistsAsync(updateDto.ContactPersonId);
            if (!contactPersonExists)
                throw new KeyNotFoundException($"Contact person with ID {updateDto.ContactPersonId} not found");
        }

        /// <summary>
        /// Получить компанию по ИНН (специфичный метод для Company)
        /// </summary>
        public async Task<CompanyDto?> GetCompanyByInnAsync(string inn)
        {
            var company = await _companyRepository.GetCompanyByInnAsync(inn);
            return company != null ? _mapper.Map<CompanyDto>(company) : null;
        }
    }
}

