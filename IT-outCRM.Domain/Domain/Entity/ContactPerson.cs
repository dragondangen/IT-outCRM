namespace IT_outCRM.Domain.Entity
{
    public class ContactPerson
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string SurName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
    }
}
