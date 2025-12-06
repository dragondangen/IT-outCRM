namespace IT_outCRM.Blazor.Services
{
    public interface IProfileService
    {
        Task<string?> UploadAvatarAsync(Stream fileStream, string fileName);
        Task<bool> DeleteAvatarAsync();
        void SetToken(string token);
    }
}


