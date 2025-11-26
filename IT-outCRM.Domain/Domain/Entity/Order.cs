using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class Order
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        /// <summary>
        /// Навигационное свойство к клиенту
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public Customer? Customer { get; set; }

        public Guid CustomerId { get; set; }

        /// <summary>
        /// Навигационное свойство к исполнителю
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public Executor? Executor { get; set; }

        public Guid? ExecutorId { get; set; }

        /// <summary>
        /// Навигационное свойство к статусу заказа
        /// </summary>
        [JsonIgnore]
        public OrderStatus? OrderStatus { get; set; }

        public Guid OrderStatusId { get; set; }

        /// <summary>
        /// Навигационное свойство к команде поддержки
        /// Может быть null, если команда поддержки еще не назначена
        /// </summary>
        [JsonIgnore]
        public OrderSupportTeam? SupportTeam { get; set; }

        public Guid? SupportTeamId { get; set; }
    }
}
