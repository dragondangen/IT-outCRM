using IT_outCRM.Application.DTOs.Common;
using IT_outCRM.Application.DTOs.ContactPerson;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class ContactPersonsController : BaseController
    {
        private readonly IContactPersonService _contactPersonService;

        public ContactPersonsController(IContactPersonService contactPersonService, ILogger<ContactPersonsController> logger)
            : base(logger)
        {
            _contactPersonService = contactPersonService;
        }

        /// <summary>
        /// Получить всех контактных лиц
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactPersonDto>>> GetAll()
        {
            var contactPersons = await _contactPersonService.GetAllAsync();
            return Ok(contactPersons);
        }

        /// <summary>
        /// Получить контактных лиц с пагинацией
        /// </summary>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<ContactPersonDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var contactPersons = await _contactPersonService.GetPagedAsync(pageNumber, pageSize);
            return Ok(contactPersons);
        }

        /// <summary>
        /// Получить контактное лицо по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactPersonDto>> GetById(Guid id)
        {
            var contactPerson = await _contactPersonService.GetByIdAsync(id);
            var notFoundResult = HandleNotFound<ContactPersonDto>(contactPerson, id, "Контактное лицо");
            if (notFoundResult != null)
                return notFoundResult;

            return Ok(contactPerson);
        }

        /// <summary>
        /// Создать новое контактное лицо
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult<ContactPersonDto>> Create([FromBody] CreateContactPersonDto createDto)
        {
            var contactPerson = await _contactPersonService.CreateAsync(createDto);
            LogCreated(contactPerson, contactPerson.Id, "Контактное лицо");
            return CreatedAtAction(nameof(GetById), new { id = contactPerson.Id }, contactPerson);
        }

        /// <summary>
        /// Обновить контактное лицо
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ContactPersonDto>> Update(Guid id, [FromBody] UpdateContactPersonDto updateDto)
        {
            var validationResult = ValidateUpdateId<ContactPersonDto>(id, updateDto.Id);
            if (validationResult != null)
                return validationResult;

            var contactPerson = await _contactPersonService.UpdateAsync(updateDto);
            LogUpdated(id, "Контактное лицо");
            return Ok(contactPerson);
        }

        /// <summary>
        /// Удалить контактное лицо
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _contactPersonService.DeleteAsync(id);
            LogDeleted(id, "Контактное лицо");
            return NoContent();
        }

        /// <summary>
        /// Найти контактное лицо по email
        /// </summary>
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<ContactPersonDto>> GetByEmail(string email)
        {
            var contactPerson = await _contactPersonService.GetByEmailAsync(email);
            if (contactPerson == null)
                return NotFound($"Контактное лицо с email {email} не найдено");

            return Ok(contactPerson);
        }
    }
}

