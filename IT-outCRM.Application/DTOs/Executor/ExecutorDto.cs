namespace IT_outCRM.Application.DTOs.Executor
{
    public class ExecutorDto
    {
        public Guid Id { get; set; }
        public int CompletedOrders { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}

