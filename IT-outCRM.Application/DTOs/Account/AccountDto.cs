namespace IT_outCRM.Application.DTOs.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime FoundingDate { get; set; }
        public Guid AccountStatusId { get; set; }
        public string AccountStatusName { get; set; } = string.Empty;
    }
}

