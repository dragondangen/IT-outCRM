using IT_outCRM.Application.DTOs.Analytics;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IAnalyticsService
    {
        Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync();
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 12);
        Task<List<MonthlyOrdersDto>> GetMonthlyOrdersAsync(int months = 12);
        Task<List<TopCustomerDto>> GetTopCustomersAsync(int count = 10);
        Task<List<ExecutorPerformanceDto>> GetExecutorPerformanceAsync();
        Task<List<PopularServiceDto>> GetPopularServicesAsync(int count = 10);
    }
}
