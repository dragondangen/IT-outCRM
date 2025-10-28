using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Inn { get; set; } = string.Empty;

        public string LegalForm { get; set; } = string.Empty;

        /// <summary>
        /// Навигационное свойство к контактному лицу
        /// JsonIgnore предотвращает циклические зависимости при сериализации
        /// </summary>
        [JsonIgnore]
        public ContactPerson? ContactPerson { get; set; }

        public Guid ContactPersonID { get; set; }
    }
}
