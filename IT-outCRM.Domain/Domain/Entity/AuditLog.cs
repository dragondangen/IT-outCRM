namespace IT_outCRM.Domain.Entity
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;  // Created, Updated, Deleted
        public string? Changes { get; set; }
        public string? UserName { get; set; }
        public Guid? UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
