namespace IT_outCRM.Application.DTOs.Executor
{
    public class CreateExecutorDto
    {
        public int CompletedOrders { get; set; }
        public Guid AccountId { get; set; }
        public Guid CompanyId { get; set; }
    }
}

