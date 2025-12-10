namespace IT_outCRM.Blazor.Models
{
    public class ExecutorModel
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int CompletedOrders { get; set; }
    }
}




