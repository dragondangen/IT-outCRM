namespace IT_outCRM.Blazor.Services
{
    public interface ITokenStorage
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        Task<bool> HasTokenAsync();
    }
}



