using IT_outCRM.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditController : BaseController
    {
        private readonly CrmDbContext _db;

        public AuditController(CrmDbContext db, ILogger<AuditController> logger) : base(logger)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? entityName = null,
            [FromQuery] Guid? entityId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            page = Math.Max(page, 1);

            var query = _db.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(entityName))
                query = query.Where(a => a.EntityName == entityName);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new
                {
                    a.Id,
                    a.EntityName,
                    a.EntityId,
                    a.Action,
                    a.Changes,
                    a.UserName,
                    a.Timestamp
                })
                .ToListAsync();

            return Ok(new { totalCount, page, pageSize, items = logs });
        }

        [HttpGet("{entityName}/{entityId}")]
        public async Task<IActionResult> GetEntityHistory(string entityName, Guid entityId)
        {
            var logs = await _db.AuditLogs
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .Take(100)
                .Select(a => new
                {
                    a.Id,
                    a.Action,
                    a.Changes,
                    a.UserName,
                    a.Timestamp
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
