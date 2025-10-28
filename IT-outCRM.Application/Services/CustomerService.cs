using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Customer;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetCustomerWithDetailsAsync(id);
            return customer != null ? _mapper.Map<CustomerDto>(customer) : null;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var totalCount = await _unitOfWork.Customers.CountAsync();

            var pagedCustomers = customers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<CustomerDto>
            {
                Items = _mapper.Map<List<CustomerDto>>(pagedCustomers),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto createDto)
        {
            // Проверка существования связанных сущностей
            if (!await _unitOfWork.Accounts.ExistsAsync(createDto.AccountId))
                throw new KeyNotFoundException($"Account with ID {createDto.AccountId} not found");

            if (!await _unitOfWork.Companies.ExistsAsync(createDto.CompanyId))
                throw new KeyNotFoundException($"Company with ID {createDto.CompanyId} not found");

            var customer = _mapper.Map<Customer>(createDto);
            customer.Id = Guid.NewGuid();

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(customer.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created customer");
        }

        public async Task<CustomerDto> UpdateAsync(UpdateCustomerDto updateDto)
        {
            var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Customer with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingCustomer);

            await _unitOfWork.Customers.UpdateAsync(existingCustomer);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingCustomer.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated customer");
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer with ID {id} not found");

            await _unitOfWork.Customers.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersByCompanyAsync(Guid companyId)
        {
            var customers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(companyId);
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }
    }
}

