using System.Reflection;
using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Базовый класс для сервисов, реализующий общие CRUD операции
    /// Устраняет дублирование кода (DRY принцип)
    /// </summary>
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> 
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected abstract IGenericRepository<TEntity> Repository { get; }

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить сущность по ID
        /// </summary>
        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id);
            return entity != null ? _mapper.Map<TDto>(entity) : null;
        }

        /// <summary>
        /// Получить все сущности
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await Repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        /// <summary>
        /// Получить сущности с пагинацией (эффективная пагинация на уровне БД)
        /// </summary>
        public virtual async Task<PagedResult<TDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await Repository.GetPagedAsync(pageNumber, pageSize);

            return new PagedResult<TDto>
            {
                Items = _mapper.Map<List<TDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Создать новую сущность
        /// </summary>
        public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
        {
            await ValidateCreateAsync(createDto);
            
            var entity = _mapper.Map<TEntity>(createDto);
            
            // Устанавливаем ID если это Guid (используем рефлексию)
            SetEntityId(entity);

            await Repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAfterCreateAsync(entity) 
                ?? throw new InvalidOperationException($"Failed to retrieve created {typeof(TEntity).Name}");
        }

        /// <summary>
        /// Обновить сущность
        /// </summary>
        public virtual async Task<TDto> UpdateAsync(TUpdateDto updateDto)
        {
            var existingEntity = await GetEntityForUpdateAsync(updateDto)
                ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {GetIdFromUpdateDto(updateDto)} not found");

            await ValidateUpdateAsync(updateDto, existingEntity);

            _mapper.Map(updateDto, existingEntity);

            await Repository.UpdateAsync(existingEntity);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAfterUpdateAsync(existingEntity)
                ?? throw new InvalidOperationException($"Failed to retrieve updated {typeof(TEntity).Name}");
        }

        /// <summary>
        /// Удалить сущность
        /// </summary>
        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found");

            await ValidateDeleteAsync(entity);

            await Repository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // Виртуальные методы для переопределения в наследниках
        protected virtual Task ValidateCreateAsync(TCreateDto createDto) => Task.CompletedTask;
        protected virtual Task ValidateUpdateAsync(TUpdateDto updateDto, TEntity existingEntity) => Task.CompletedTask;
        protected virtual Task ValidateDeleteAsync(TEntity entity) => Task.CompletedTask;

        protected virtual Task<TEntity?> GetEntityForUpdateAsync(TUpdateDto updateDto)
        {
            var id = GetIdFromUpdateDto(updateDto);
            return Repository.GetByIdAsync(id);
        }

        protected virtual Task<TDto?> GetByIdAfterCreateAsync(TEntity entity)
        {
            var id = GetEntityId(entity);
            if (id.HasValue)
            {
                return GetByIdAsync(id.Value);
            }
            return Task.FromResult<TDto?>(null);
        }

        protected virtual Task<TDto?> GetByIdAfterUpdateAsync(TEntity entity)
        {
            var id = GetEntityId(entity);
            if (id.HasValue)
            {
                return GetByIdAsync(id.Value);
            }
            return Task.FromResult<TDto?>(null);
        }

        protected virtual Guid? GetEntityId(TEntity entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty?.GetValue(entity) is Guid id)
            {
                return id;
            }
            return null;
        }

        protected virtual void SetEntityId(TEntity entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(Guid))
            {
                var currentId = idProperty.GetValue(entity);
                if (currentId == null || (Guid)currentId == Guid.Empty)
                {
                    idProperty.SetValue(entity, Guid.NewGuid());
                }
            }
        }

        protected virtual Guid GetIdFromUpdateDto(TUpdateDto updateDto)
        {
            // Предполагаем, что все UpdateDto имеют свойство Id типа Guid
            var idProperty = typeof(TUpdateDto).GetProperty("Id");
            if (idProperty?.GetValue(updateDto) is Guid id)
            {
                return id;
            }
            throw new InvalidOperationException($"UpdateDto must have Id property of type Guid");
        }
    }
}

