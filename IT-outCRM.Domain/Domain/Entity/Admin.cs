namespace IT_outCRM.Domain.Entity
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
    }
}
