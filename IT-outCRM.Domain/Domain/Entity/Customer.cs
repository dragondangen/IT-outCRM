namespace IT_outCRM.Domain.Entity
{
    public class Customer
    {
        public Guid Id { get; set; }

        public Account? Account { get; set; }

        public Guid AccountId { get; set; }

        public Company? Company { get; set; }

        public Guid CompanyId { get; set; }
    }
}
