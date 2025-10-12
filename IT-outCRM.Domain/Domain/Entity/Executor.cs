namespace IT_outCRM.Domain.Entity
{
    public class Executor
    {
        public Guid Id { get; private set; }
        public int CompletedOrders { get; private set; }
        public Account? Account { get; private set; }
        public Guid AccountId { get; private set; }
    }
}
