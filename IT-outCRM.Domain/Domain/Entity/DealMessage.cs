using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class DealMessage
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Deal? Deal { get; set; }
        public Guid DealId { get; set; }

        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
