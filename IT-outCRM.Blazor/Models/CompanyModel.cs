namespace IT_outCRM.Blazor.Models
{
    public class CompanyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Новые поля
        public string LegalForm { get; set; } = string.Empty;
        public Guid ContactPersonId { get; set; }
    }
}
