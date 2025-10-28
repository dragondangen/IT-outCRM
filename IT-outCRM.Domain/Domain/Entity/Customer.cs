using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class Customer
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Навигационное свойство к аккаунту
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public Account? Account { get; set; }

        public Guid AccountId { get; set; }

        /// <summary>
        /// Навигационное свойство к компании
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public Company? Company { get; set; }

        public Guid CompanyId { get; set; }
    }
}
