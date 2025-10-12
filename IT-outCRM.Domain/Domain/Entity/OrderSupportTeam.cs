namespace IT_outCRM.Domain.Entity
{
    public class OrderSupportTeam
    {
        public Guid Id { get; private set; }
        public Admin? Admin { get; private set; }
        public Guid AdminId { get; private set; }
    }
}
