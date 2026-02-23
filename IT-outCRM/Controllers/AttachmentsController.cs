using System.Security.Claims;
using IT_outCRM.Domain.Entity;
using IT_outCRM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentsController : BaseController
    {
        private readonly CrmDbContext _db;
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv",
            ".png", ".jpg", ".jpeg", ".gif", ".webp", ".zip", ".rar"
        };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public AttachmentsController(CrmDbContext db, IWebHostEnvironment env, ILogger<AttachmentsController> logger)
            : base(logger)
        {
            _db = db;
            _env = env;
        }

        /// <summary>
        /// Загрузить файл к сущности (order/deal)
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(10_485_760)]
        public async Task<IActionResult> Upload([FromForm] string entityType, [FromForm] Guid entityId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            if (file.Length > MaxFileSize)
                return BadRequest($"Максимальный размер файла — {MaxFileSize / 1024 / 1024} МБ");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                return BadRequest($"Недопустимый формат файла: {ext}");

            if (entityType != "order" && entityType != "deal")
                return BadRequest("entityType должен быть 'order' или 'deal'");

            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "attachments", entityType);
            Directory.CreateDirectory(uploadsDir);

            var storedName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, storedName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                FileName = file.FileName,
                StoredFileName = storedName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedByUserId = userId.Value,
                UploadedAt = DateTime.UtcNow
            };

            _db.Attachments.Add(attachment);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                attachment.Id,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType,
                attachment.UploadedAt,
                DownloadUrl = $"/api/attachments/{attachment.Id}/download"
            });
        }

        /// <summary>
        /// Получить вложения сущности
        /// </summary>
        [HttpGet("{entityType}/{entityId}")]
        public async Task<IActionResult> GetByEntity(string entityType, Guid entityId)
        {
            var attachments = await _db.Attachments
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.UploadedAt)
                .Select(a => new
                {
                    a.Id,
                    a.FileName,
                    a.FileSize,
                    a.ContentType,
                    a.UploadedAt,
                    DownloadUrl = $"/api/attachments/{a.Id}/download"
                })
                .ToListAsync();

            return Ok(attachments);
        }

        /// <summary>
        /// Скачать вложение
        /// </summary>
        [HttpGet("{id}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> Download(Guid id)
        {
            var attachment = await _db.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", "attachments",
                attachment.EntityType, attachment.StoredFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Файл не найден на диске");

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, attachment.ContentType, attachment.FileName);
        }

        /// <summary>
        /// Удалить вложение
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var attachment = await _db.Attachments.FindAsync(id);
            if (attachment == null) return NotFound();

            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (attachment.UploadedByUserId != userId.Value && !User.IsInRole("Admin"))
                return Forbid();

            var filePath = Path.Combine(_env.WebRootPath, "uploads", "attachments",
                attachment.EntityType, attachment.StoredFileName);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.Attachments.Remove(attachment);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private Guid? GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idStr, out var id) ? id : null;
        }
    }
}
