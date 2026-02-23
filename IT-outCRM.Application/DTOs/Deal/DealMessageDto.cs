namespace IT_outCRM.Application.DTOs.Deal
{
    public class DealMessageDto
    {
        public Guid Id { get; set; }
        public Guid DealId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
