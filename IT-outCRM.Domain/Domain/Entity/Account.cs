namespace IT_outCRM.Domain.Entity
{
    public class Account
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public DateTime FoundingDate { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public int AccountStatusId { get; set; }
    }
}
