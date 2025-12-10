using AutoMapper;
using IT_outCRM.Application.DTOs.Customer;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с клиентами
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class CustomerService : BaseService<Customer, CustomerDto, CreateCustomerDto, UpdateCustomerDto>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IEntityValidationService _validationService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IEntityValidationService validationService) 
            : base(unitOfWork, mapper)
        {
            _customerRepository = unitOfWork.Customers;
            _validationService = validationService;
        }

        protected override IGenericRepository<Customer> Repository => _customerRepository;

        /// <summary>
        /// Переопределяем для загрузки связанных сущностей
        /// </summary>
        public override async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetCustomerWithDetailsAsync(id);
            return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<CustomerDto?> GetByIdAfterCreateAsync(Customer entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<CustomerDto?>(null);
        }

        protected override Task<CustomerDto?> GetByIdAfterUpdateAsync(Customer entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<CustomerDto?>(null);
        }

        /// <summary>
        /// Валидация при создании - проверка существования связанных сущностей
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateCustomerDto createDto)
        {
            await _validationService.EnsureAccountExistsAsync(createDto.AccountId);
            await _validationService.EnsureCompanyExistsAsync(createDto.CompanyId);
        }

        /// <summary>
        /// Переопределяем GetAllAsync для загрузки связанных сущностей
        /// </summary>
        public override async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        /// <summary>
        /// Получить клиентов по компании (специфичный метод для Customer)
        /// </summary>
        public async Task<IEnumerable<CustomerDto>> GetCustomersByCompanyAsync(Guid companyId)
        {
            var customers = await _customerRepository.GetCustomersByCompanyAsync(companyId);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}

