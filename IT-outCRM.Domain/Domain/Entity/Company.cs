namespace IT_outCRM.Domain.Entity
{
    public class Company
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Inn { get; private set; } = string.Empty;
        public string LegalForm { get; private set; } = string.Empty;
    }
}
