namespace IT_outCRM.Domain.Entity
{
    public class Attachment
    {
        public Guid Id { get; set; }

        /// <summary>order, deal, or message</summary>
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public Guid UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public User? UploadedByUser { get; set; }
    }
}
