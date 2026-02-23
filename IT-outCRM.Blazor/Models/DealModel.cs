namespace IT_outCRM.Blazor.Models
{
    public class DealModel
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderName { get; set; } = string.Empty;
        public string OrderStatusName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid ExecutorId { get; set; }
        public string ExecutorName { get; set; } = string.Empty;
        public Guid? ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal AgreedPrice { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Terms { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CustomerRating { get; set; }
        public string? CustomerReview { get; set; }
        public int? ExecutorRating { get; set; }
        public string? ExecutorReview { get; set; }
        public List<DealMessageModel> Messages { get; set; } = new();
    }

    public class DealMessageModel
    {
        public Guid Id { get; set; }
        public Guid DealId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDealFromOrderModel
    {
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ExecutorId { get; set; }
        public Guid? ServiceId { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Terms { get; set; }
    }
}
