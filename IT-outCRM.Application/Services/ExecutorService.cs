using AutoMapper;
using IT_outCRM.Application.DTOs.Executor;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с исполнителями
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class ExecutorService : BaseService<Executor, ExecutorDto, CreateExecutorDto, UpdateExecutorDto>, IExecutorService
    {
        private readonly IExecutorRepository _executorRepository;
        private readonly IEntityValidationService _validationService;

        public ExecutorService(IUnitOfWork unitOfWork, IMapper mapper, IEntityValidationService validationService) 
            : base(unitOfWork, mapper)
        {
            _executorRepository = unitOfWork.Executors;
            _validationService = validationService;
        }

        protected override IGenericRepository<Executor> Repository => _executorRepository;

        /// <summary>
        /// Переопределяем для загрузки связанных сущностей
        /// </summary>
        public override async Task<ExecutorDto?> GetByIdAsync(Guid id)
        {
            var executor = await _executorRepository.GetExecutorWithDetailsAsync(id);
            return executor != null ? _mapper.Map<ExecutorDto>(executor) : null;
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<ExecutorDto?> GetByIdAfterCreateAsync(Executor entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<ExecutorDto?>(null);
        }

        protected override Task<ExecutorDto?> GetByIdAfterUpdateAsync(Executor entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<ExecutorDto?>(null);
        }

        /// <summary>
        /// Валидация при создании - проверка существования связанных сущностей
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateExecutorDto createDto)
        {
            await _validationService.EnsureAccountExistsAsync(createDto.AccountId);
            await _validationService.EnsureCompanyExistsAsync(createDto.CompanyId);
            
            // Проверяем, что для этого Account не существует Customer
            // Один Account не может быть одновременно Customer и Executor
            var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(createDto.CompanyId);
            var customerWithSameAccount = customers.FirstOrDefault(c => c.AccountId == createDto.AccountId);
            if (customerWithSameAccount != null)
            {
                throw new InvalidOperationException($"Для аккаунта {createDto.AccountId} уже существует Customer (ID: {customerWithSameAccount.Id}). Нельзя создать Executor для аккаунта с Customer. Сначала удалите Customer или используйте другой аккаунт.");
            }
            
            // Проверяем, что Executor еще не существует для этого Account и Company
            var existingExecutors = await _unitOfWork.Executors.GetAllAsync();
            var executorWithSameAccount = existingExecutors.FirstOrDefault(e => e.AccountId == createDto.AccountId && e.CompanyId == createDto.CompanyId);
            if (executorWithSameAccount != null)
            {
                throw new InvalidOperationException($"Для аккаунта {createDto.AccountId} и компании {createDto.CompanyId} уже существует Executor (ID: {executorWithSameAccount.Id}).");
            }
        }

        /// <summary>
        /// Получить топ исполнителей (специфичный метод для Executor)
        /// </summary>
        public async Task<IEnumerable<ExecutorDto>> GetTopExecutorsAsync(int count)
        {
            var executors = await _executorRepository.GetTopExecutorsAsync(count);
            return _mapper.Map<IEnumerable<ExecutorDto>>(executors);
        }
    }
}

