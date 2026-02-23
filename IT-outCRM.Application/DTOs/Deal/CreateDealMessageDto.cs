namespace IT_outCRM.Application.DTOs.Deal
{
    public class CreateDealMessageDto
    {
        public Guid DealId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
