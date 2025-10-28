namespace IT_outCRM.Application.DTOs.Executor
{
    public class UpdateExecutorDto
    {
        public Guid Id { get; set; }
        public int CompletedOrders { get; set; }
        public Guid AccountId { get; set; }
        public Guid CompanyId { get; set; }
    }
}

