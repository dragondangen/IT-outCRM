namespace IT_outCRM.Domain.Entity
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public DateTime FoundingDate { get; private set; }
        public AccountStatus? AccountStatus { get; private set; }
        public int AccountStatusId { get;private set; }
    }
}
