using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class Deal
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }
        public Guid OrderId { get; set; }

        [JsonIgnore]
        public Customer? Customer { get; set; }
        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public Executor? Executor { get; set; }
        public Guid ExecutorId { get; set; }

        [JsonIgnore]
        public Service? Service { get; set; }
        public Guid? ServiceId { get; set; }

        public decimal AgreedPrice { get; set; }

        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Новая → Предложена → Согласована → В работе → На проверке → Завершена / Отменена
        /// </summary>
        public string Status { get; set; } = "Новая";

        public string? Terms { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int? CustomerRating { get; set; }
        public string? CustomerReview { get; set; }

        public int? ExecutorRating { get; set; }
        public string? ExecutorReview { get; set; }

        [JsonIgnore]
        public ICollection<DealMessage>? Messages { get; set; }
    }
}
