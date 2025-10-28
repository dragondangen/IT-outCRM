using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class Account
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public DateTime FoundingDate { get; set; }

        /// <summary>
        /// Навигационное свойство к статусу
        /// </summary>
        [JsonIgnore]
        public AccountStatus? AccountStatus { get; set; }

        public Guid AccountStatusId { get; set; }
    }
}
