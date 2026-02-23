namespace IT_outCRM.Blazor.Services
{
    public interface IProfileService
    {
        Task<string?> UploadAvatarAsync(Stream fileStream, string fileName);
        Task<bool> DeleteAvatarAsync();
        Task<ProfileInfo?> GetMyInfoAsync();
        void SetToken(string token);
    }

    public class ProfileInfo
    {
        public string CompanyName { get; set; } = "";
        public string CompanyInn { get; set; } = "";
        public string CompanyLegalForm { get; set; } = "";
        public string ContactFirstName { get; set; } = "";
        public string ContactLastName { get; set; } = "";
        public string ContactPhone { get; set; } = "";
        public string ContactRole { get; set; } = "";
        public string? AccountName { get; set; }
        public string? AccountStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ExecutorId { get; set; }
        public int CompletedOrders { get; set; }
    }
}


