namespace IT_outCRM.Blazor.Models
{
    public class ExecutorModel
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }
}



