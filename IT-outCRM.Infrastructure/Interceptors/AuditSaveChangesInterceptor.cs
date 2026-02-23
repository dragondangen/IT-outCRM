using System.Text.Json;
using IT_outCRM.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace IT_outCRM.Infrastructure.Interceptors
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        private static readonly HashSet<string> AuditedEntities = new()
        {
            nameof(Order), nameof(Deal), nameof(Customer), nameof(Executor),
            nameof(Account), nameof(Company), nameof(Service)
        };

        public AuditSaveChangesInterceptor(IHttpContextAccessor? httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is CrmDbContext db)
                AddAuditEntries(db);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AddAuditEntries(CrmDbContext db)
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var userName = user?.FindFirst(ClaimTypes.Name)?.Value
                        ?? user?.FindFirst(ClaimTypes.Email)?.Value;
            var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out var userId);

            foreach (var entry in db.ChangeTracker.Entries()
                .Where(e => AuditedEntities.Contains(e.Entity.GetType().Name) &&
                           e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
            {
                var entityName = entry.Entity.GetType().Name;
                var idProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
                var entityId = idProp?.CurrentValue is Guid g ? g : Guid.Empty;

                string action = entry.State switch
                {
                    EntityState.Added => "Created",
                    EntityState.Modified => "Updated",
                    EntityState.Deleted => "Deleted",
                    _ => "Unknown"
                };

                string? changes = null;
                if (entry.State == EntityState.Modified)
                {
                    var changedProps = entry.Properties
                        .Where(p => p.IsModified && p.Metadata.Name != "Id")
                        .Select(p => new
                        {
                            Property = p.Metadata.Name,
                            OldValue = p.OriginalValue?.ToString(),
                            NewValue = p.CurrentValue?.ToString()
                        })
                        .ToList();

                    if (changedProps.Count > 0)
                        changes = JsonSerializer.Serialize(changedProps);
                }

                db.AuditLogs.Add(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = entityName,
                    EntityId = entityId,
                    Action = action,
                    Changes = changes,
                    UserName = userName,
                    UserId = userId != Guid.Empty ? userId : null,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
