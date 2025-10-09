namespace IT_outCRM.Domain.Entity
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
    }
}
