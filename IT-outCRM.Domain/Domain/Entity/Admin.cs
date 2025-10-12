namespace IT_outCRM.Domain.Entity
{
    public class Admin
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Account? Account { get; private set; }
        public Guid AccountId { get; private set; }
    }
}
