using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Executor;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class ExecutorService : IExecutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExecutorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ExecutorDto?> GetByIdAsync(Guid id)
        {
            var executor = await _unitOfWork.Executors.GetExecutorWithDetailsAsync(id);
            return executor != null ? _mapper.Map<ExecutorDto>(executor) : null;
        }

        public async Task<IEnumerable<ExecutorDto>> GetAllAsync()
        {
            var executors = await _unitOfWork.Executors.GetAllAsync();
            return _mapper.Map<IEnumerable<ExecutorDto>>(executors);
        }

        public async Task<PagedResult<ExecutorDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var executors = await _unitOfWork.Executors.GetAllAsync();
            var totalCount = await _unitOfWork.Executors.CountAsync();

            var pagedExecutors = executors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<ExecutorDto>
            {
                Items = _mapper.Map<List<ExecutorDto>>(pagedExecutors),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ExecutorDto> CreateAsync(CreateExecutorDto createDto)
        {
            // Проверка существования связанных сущностей
            if (!await _unitOfWork.Accounts.ExistsAsync(createDto.AccountId))
                throw new KeyNotFoundException($"Account with ID {createDto.AccountId} not found");

            if (!await _unitOfWork.Companies.ExistsAsync(createDto.CompanyId))
                throw new KeyNotFoundException($"Company with ID {createDto.CompanyId} not found");

            var executor = _mapper.Map<Executor>(createDto);
            executor.Id = Guid.NewGuid();

            await _unitOfWork.Executors.AddAsync(executor);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(executor.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created executor");
        }

        public async Task<ExecutorDto> UpdateAsync(UpdateExecutorDto updateDto)
        {
            var existingExecutor = await _unitOfWork.Executors.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Executor with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingExecutor);

            await _unitOfWork.Executors.UpdateAsync(existingExecutor);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingExecutor.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated executor");
        }

        public async Task DeleteAsync(Guid id)
        {
            var executor = await _unitOfWork.Executors.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Executor with ID {id} not found");

            await _unitOfWork.Executors.DeleteAsync(executor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExecutorDto>> GetTopExecutorsAsync(int count)
        {
            var executors = await _unitOfWork.Executors.GetTopExecutorsAsync(count);
            return _mapper.Map<IEnumerable<ExecutorDto>>(executors);
        }
    }
}

