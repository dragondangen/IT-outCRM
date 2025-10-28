namespace IT_outCRM.Application.DTOs.Account
{
    public class UpdateAccountDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime FoundingDate { get; set; }
        public Guid AccountStatusId { get; set; }
    }
}

