namespace IT_outCRM.Application.DTOs.Company
{
    public class UpdateCompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string LegalForm { get; set; } = string.Empty;
        public Guid ContactPersonId { get; set; }
    }
}

