namespace IT_outCRM.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        // Nullable для поддержки автоматического заполнения для клиентов
        public Guid? CustomerId { get; set; }
        public Guid? ExecutorId { get; set; }
        // Nullable для поддержки автоматического заполнения для клиентов
        public Guid? OrderStatusId { get; set; }
        // Nullable для поддержки автоматического заполнения для клиентов
        public Guid? SupportTeamId { get; set; }
    }
}

