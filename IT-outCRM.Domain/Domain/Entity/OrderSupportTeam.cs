namespace IT_outCRM.Domain.Entity
{
    public class OrderSupportTeam
    {
        public int Id { get; set; }
        public Admin Admin { get; set; }
        public Guid AdminId { get; set; }
    }
}
