namespace IT_outCRM.Application.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid ExecutorId { get; set; }
        public string ExecutorName { get; set; } = string.Empty;
        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; } = string.Empty;
        public Guid SupportTeamId { get; set; }
    }
}

