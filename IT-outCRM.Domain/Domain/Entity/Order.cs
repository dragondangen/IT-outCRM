namespace IT_outCRM.Domain.Entity
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public Customer? Customer { get; private set; }
        public Guid CustomerId { get; private set; }
        public Executor? Executor { get; private set; }
        public Guid ExecutorId { get; private set; }
        public OrderStatus? OrderStatus { get; private set; }
        public int OrderStatusId { get; private set; }
    }
}
