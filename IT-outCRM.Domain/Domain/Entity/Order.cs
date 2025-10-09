namespace IT_outCRM.Domain.Entity
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public Executor Executor { get; set; }
        public Guid ExecutorId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int OrderStatusId { get; set; }
    }
}
