using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public class AccountStatusService : IAccountStatusService
    {
        private readonly HttpClient _httpClient;

        public AccountStatusService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AccountStatusModel>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<AccountStatusModel>>("api/accountstatuses");
                return result ?? new List<AccountStatusModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading account statuses: {ex.Message}");
                return new List<AccountStatusModel>();
            }
        }
    }
}



