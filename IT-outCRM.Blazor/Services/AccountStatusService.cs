using System.Net.Http.Json;
using IT_outCRM.Blazor.Models;
using Microsoft.Extensions.Logging;

namespace IT_outCRM.Blazor.Services
{
    public class AccountStatusService : IAccountStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountStatusService> _logger;

        public AccountStatusService(HttpClient httpClient, ILogger<AccountStatusService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
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
                _logger.LogWarning(ex, "Error loading account statuses");
                return new List<AccountStatusModel>();
            }
        }
    }
}




