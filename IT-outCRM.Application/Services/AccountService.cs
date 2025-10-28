using AutoMapper;
using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AccountDto?> GetByIdAsync(Guid id)
        {
            var account = await _unitOfWork.Accounts.GetAccountWithStatusAsync(id);
            return account != null ? _mapper.Map<AccountDto>(account) : null;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            var accounts = await _unitOfWork.Accounts.GetAllAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        public async Task<PagedResult<AccountDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var accounts = await _unitOfWork.Accounts.GetAllAsync();
            var totalCount = await _unitOfWork.Accounts.CountAsync();

            var pagedAccounts = accounts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<AccountDto>
            {
                Items = _mapper.Map<List<AccountDto>>(pagedAccounts),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<AccountDto> CreateAsync(CreateAccountDto createDto)
        {
            var account = _mapper.Map<Account>(createDto);
            account.Id = Guid.NewGuid();

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(account.Id) 
                ?? throw new InvalidOperationException("Failed to retrieve created account");
        }

        public async Task<AccountDto> UpdateAsync(UpdateAccountDto updateDto)
        {
            var existingAccount = await _unitOfWork.Accounts.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Account with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingAccount);

            await _unitOfWork.Accounts.UpdateAsync(existingAccount);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingAccount.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated account");
        }

        public async Task DeleteAsync(Guid id)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Account with ID {id} not found");

            await _unitOfWork.Accounts.DeleteAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsByStatusAsync(Guid statusId)
        {
            var accounts = await _unitOfWork.Accounts.GetAccountsByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }
    }
}

