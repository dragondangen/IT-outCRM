using AutoMapper;
using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    /// <summary>
    /// Сервис для работы с аккаунтами
    /// Наследуется от BaseService для устранения дублирования кода (DRY)
    /// </summary>
    public class AccountService : BaseService<Account, AccountDto, CreateAccountDto, UpdateAccountDto>, IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
            _accountRepository = unitOfWork.Accounts;
        }

        protected override IGenericRepository<Account> Repository => _accountRepository;

        /// <summary>
        /// Переопределяем для загрузки связанной сущности AccountStatus
        /// </summary>
        public override async Task<AccountDto?> GetByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetAccountWithStatusAsync(id);
            return account != null ? _mapper.Map<AccountDto>(account) : null;
        }

        /// <summary>
        /// Переопределяем для загрузки связанной сущности AccountStatus
        /// </summary>
        public override async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            var accounts = await _accountRepository.GetAllWithStatusAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        /// <summary>
        /// Переопределяем для загрузки связанной сущности AccountStatus
        /// </summary>
        public override async Task<PagedResult<AccountDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _accountRepository.GetPagedWithStatusAsync(pageNumber, pageSize);

            return new PagedResult<AccountDto>
            {
                Items = _mapper.Map<List<AccountDto>>(items),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Переопределяем GetByIdAfterCreateAsync и GetByIdAfterUpdateAsync для использования правильного метода
        /// </summary>
        protected override Task<AccountDto?> GetByIdAfterCreateAsync(Account entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<AccountDto?>(null);
        }

        protected override Task<AccountDto?> GetByIdAfterUpdateAsync(Account entity)
        {
            var id = GetEntityId(entity);
            return id.HasValue ? GetByIdAsync(id.Value) : Task.FromResult<AccountDto?>(null);
        }

        /// <summary>
        /// Получить аккаунты по статусу (специфичный метод для Account)
        /// </summary>
        public async Task<IEnumerable<AccountDto>> GetAccountsByStatusAsync(Guid statusId)
        {
            var accounts = await _accountRepository.GetAccountsByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }
    }
}

