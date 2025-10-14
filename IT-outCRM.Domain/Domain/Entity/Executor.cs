namespace IT_outCRM.Domain.Entity
{
    public class Executor
    {
        public Guid Id { get; set; }

        public int CompletedOrders { get; set; }

        public Account? Account { get; set; }

        public Guid AccountId { get; set; }

        public Company? Company { get; set; }

        public Guid CompanyId { get; set; }
    }
}
