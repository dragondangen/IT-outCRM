namespace IT_outCRM.Blazor.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; } // Добавлено для связывания
        public string CompanyName { get; set; } = string.Empty;
        public DateTime FoundingDate { get; set; } = DateTime.Now; // Значение по умолчанию
        public Guid AccountStatusId { get; set; }
        public string AccountStatusName { get; set; } = string.Empty;
    }
}
