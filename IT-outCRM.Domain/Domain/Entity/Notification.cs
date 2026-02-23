namespace IT_outCRM.Domain.Entity
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";  // info, success, warning, error
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
