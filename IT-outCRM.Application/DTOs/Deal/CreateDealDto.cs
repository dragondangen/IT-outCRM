namespace IT_outCRM.Application.DTOs.Deal
{
    public class CreateDealDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ExecutorId { get; set; }
        public Guid? ServiceId { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Terms { get; set; }
    }
}
