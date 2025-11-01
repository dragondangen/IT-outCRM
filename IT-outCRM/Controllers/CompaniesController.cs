using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : BaseController
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
            : base(logger)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Получить все компании
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }

        /// <summary>
        /// Получить компании с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<CompanyDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var companies = await _companyService.GetPagedAsync(pageNumber, pageSize);
            return Ok(companies);
        }

        /// <summary>
        /// Получить компанию по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetById(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<CompanyDto>(company, id, "Компания");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(company);
        }

        /// <summary>
        /// Создать новую компанию
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto createDto)
        {
            var company = await _companyService.CreateAsync(createDto);
            LogCreated(company, company.Id, "Компания");
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        /// <summary>
        /// Обновить компанию
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyDto>> Update(Guid id, [FromBody] UpdateCompanyDto updateDto)
        {
            var validationResult = ValidateUpdateId<CompanyDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var company = await _companyService.UpdateAsync(updateDto);
            LogUpdated(id, "Компания");
            return Ok(company);
        }

        /// <summary>
        /// Удалить компанию
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _companyService.DeleteAsync(id);
            LogDeleted(id, "Компания");
            return NoContent();
        }

        /// <summary>
        /// Найти компанию по ИНН
        /// </summary>
        [HttpGet("by-inn/{inn}")]
        public async Task<ActionResult<CompanyDto>> GetByInn(string inn)
        {
            var company = await _companyService.GetCompanyByInnAsync(inn);
            if (company == null)
                return NotFound($"Компания с ИНН {inn} не найдена");

            return Ok(company);
        }
    }
}

