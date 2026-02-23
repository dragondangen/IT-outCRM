namespace IT_outCRM.Application.DTOs.Deal
{
    public class UpdateDealDto
    {
        public Guid Id { get; set; }
        public decimal AgreedPrice { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Terms { get; set; }
    }
}
