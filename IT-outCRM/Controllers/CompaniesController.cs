using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
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
            if (company == null)
                return NotFound($"Компания с ID {id} не найдена");

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
            _logger.LogInformation("Создана компания: {CompanyName} (ИНН: {Inn})", company.Name, company.Inn);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        /// <summary>
        /// Обновить компанию
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyDto>> Update(Guid id, [FromBody] UpdateCompanyDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("ID в URL не совпадает с ID в теле запроса");

            var company = await _companyService.UpdateAsync(updateDto);
            _logger.LogInformation("Обновлена компания: {CompanyId}", id);
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
            _logger.LogInformation("Удалена компания: {CompanyId}", id);
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

