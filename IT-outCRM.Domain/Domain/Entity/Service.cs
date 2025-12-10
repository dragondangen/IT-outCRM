using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    /// <summary>
    /// Услуга, которую предлагает исполнитель
    /// </summary>
    public class Service
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Duration { get; set; } = 1; // в часах

        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Навигационное свойство к исполнителю
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public Executor? Executor { get; set; }

        public Guid ExecutorId { get; set; }
    }
}

