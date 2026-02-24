using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IAnalyticsService
    {
        Task<DashboardAnalyticsModel?> GetDashboardAnalyticsAsync();
        void SetToken(string token);
    }
}
