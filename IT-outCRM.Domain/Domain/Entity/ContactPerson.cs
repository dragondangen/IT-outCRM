using System.Text.Json.Serialization;

namespace IT_outCRM.Domain.Entity
{
    public class ContactPerson
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Отчество (Patronymic)
        /// </summary>
        public string MiddleName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя (вычисляемое свойство)
        /// </summary>
        [JsonIgnore]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
