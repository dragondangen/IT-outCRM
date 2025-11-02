using AutoMapper;
using IT_outCRM.Application.DTOs.AccountStatus;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class AccountStatusService : BaseService<AccountStatus, AccountStatusDto, CreateAccountStatusDto, UpdateAccountStatusDto>, IAccountStatusService
    {
        private readonly IAccountStatusRepository _accountStatusRepository;

        public AccountStatusService(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            _accountStatusRepository = unitOfWork.AccountStatuses;
        }

        protected override IGenericRepository<AccountStatus> Repository => _accountStatusRepository;

        protected override async Task ValidateCreateAsync(CreateAccountStatusDto createDto)
        {
            // Проверка уникальности названия
            if (await _accountStatusRepository.NameExistsAsync(createDto.Name))
            {
                throw new InvalidOperationException($"Статус с названием '{createDto.Name}' уже существует");
            }
        }

        protected override async Task ValidateUpdateAsync(UpdateAccountStatusDto updateDto, AccountStatus existingEntity)
        {
            // Проверка уникальности названия (исключая текущую запись)
            if (await _accountStatusRepository.NameExistsAsync(updateDto.Name, updateDto.Id))
            {
                throw new InvalidOperationException($"Статус с названием '{updateDto.Name}' уже существует");
            }
        }
    }
}

