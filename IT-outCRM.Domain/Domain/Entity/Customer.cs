namespace IT_outCRM.Domain.Entity
{
    public class Customer
    {
        public Guid Id { get;private set; }
        public Account? Account { get; private set; }
        public Guid AccountId { get; private set; }
    }
}
