namespace IT_outCRM.Domain.Entity
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Inn { get; set; } = string.Empty;

        public string LegalForm { get; set; } = string.Empty;

        public ContactPerson? ContactPerson { get; set; }

        public Guid ContactPersonID { get; set; }
    }
}
