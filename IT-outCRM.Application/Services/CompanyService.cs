using AutoMapper;
using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CompanyDto?> GetByIdAsync(Guid id)
        {
            var company = await _unitOfWork.Companies.GetCompanyWithContactPersonAsync(id);
            return company != null ? _mapper.Map<CompanyDto>(company) : null;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _unitOfWork.Companies.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<PagedResult<CompanyDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var companies = await _unitOfWork.Companies.GetAllAsync();
            var totalCount = await _unitOfWork.Companies.CountAsync();

            var pagedCompanies = companies
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<CompanyDto>
            {
                Items = _mapper.Map<List<CompanyDto>>(pagedCompanies),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto createDto)
        {
            // Проверка на дубликат ИНН
            var existingCompany = await _unitOfWork.Companies.GetCompanyByInnAsync(createDto.Inn);
            if (existingCompany != null)
                throw new InvalidOperationException($"Company with INN {createDto.Inn} already exists");

            var company = _mapper.Map<Company>(createDto);
            company.Id = Guid.NewGuid();

            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(company.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created company");
        }

        public async Task<CompanyDto> UpdateAsync(UpdateCompanyDto updateDto)
        {
            var existingCompany = await _unitOfWork.Companies.GetByIdAsync(updateDto.Id)
                ?? throw new KeyNotFoundException($"Company with ID {updateDto.Id} not found");

            // Проверка на дубликат ИНН (если ИНН изменен)
            if (existingCompany.Inn != updateDto.Inn)
            {
                var companyWithSameInn = await _unitOfWork.Companies.GetCompanyByInnAsync(updateDto.Inn);
                if (companyWithSameInn != null)
                    throw new InvalidOperationException($"Company with INN {updateDto.Inn} already exists");
            }

            _mapper.Map(updateDto, existingCompany);

            await _unitOfWork.Companies.UpdateAsync(existingCompany);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(existingCompany.Id)
                ?? throw new InvalidOperationException("Failed to retrieve updated company");
        }

        public async Task DeleteAsync(Guid id)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Company with ID {id} not found");

            await _unitOfWork.Companies.DeleteAsync(company);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CompanyDto?> GetCompanyByInnAsync(string inn)
        {
            var company = await _unitOfWork.Companies.GetCompanyByInnAsync(inn);
            return company != null ? _mapper.Map<CompanyDto>(company) : null;
        }
    }
}

