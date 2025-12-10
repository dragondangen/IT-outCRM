using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Service;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с услугами
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class ServiceService : BaseService<Service, ServiceDto, CreateServiceDto, UpdateServiceDto>, IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IEntityValidationService _validationService;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, IEntityValidationService validationService) 
            : base(unitOfWork, mapper)
        {
            _serviceRepository = unitOfWork.Services;
            _validationService = validationService;
        }

        protected override IGenericRepository<Service> Repository => _serviceRepository;

        /// <summary>
        /// Переопределяем для загрузки связанных сущностей
        /// </summary>
        public override async Task<ServiceDto?> GetByIdAsync(Guid id)
        {
            var service = await _serviceRepository.GetServiceWithDetailsAsync(id);
            return service != null ? _mapper.Map<ServiceDto>(service) : null;
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<ServiceDto?> GetByIdAfterCreateAsync(Service entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<ServiceDto?>(null);
        }

        protected override Task<ServiceDto?> GetByIdAfterUpdateAsync(Service entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<ServiceDto?>(null);
        }

        /// <summary>
        /// Валидация при создании - проверка существования исполнителя
        /// </summary>
        protected override async Task ValidateCreateAsync(CreateServiceDto createDto)
        {
            await _validationService.EnsureExecutorExistsAsync(createDto.ExecutorId);
        }

        /// <summary>
        /// Переопределяем GetAllAsync для загрузки связанных сущностей
        /// </summary>
        public override async Task<IEnumerable<ServiceDto>> GetAllAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        /// <summary>
        /// Получить услуги по исполнителю
        /// </summary>
        public async Task<IEnumerable<ServiceDto>> GetServicesByExecutorAsync(Guid executorId)
        {
            var services = await _serviceRepository.GetServicesByExecutorAsync(executorId);
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        /// <summary>
        /// Получить активные услуги
        /// </summary>
        public async Task<IEnumerable<ServiceDto>> GetActiveServicesAsync()
        {
            var services = await _serviceRepository.GetActiveServicesAsync();
            return _mapper.Map<IEnumerable<ServiceDto>>(services);
        }
    }
}

