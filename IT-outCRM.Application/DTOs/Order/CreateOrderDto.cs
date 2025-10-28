namespace IT_outCRM.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ExecutorId { get; set; }
        public Guid OrderStatusId { get; set; }
        public Guid SupportTeamId { get; set; }
    }
}

