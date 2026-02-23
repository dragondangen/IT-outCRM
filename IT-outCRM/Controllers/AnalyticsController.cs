using IT_outCRM.Application.DTOs.Analytics;
using IT_outCRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class AnalyticsController : BaseController
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
            : base(logger)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<DashboardAnalyticsDto>> GetDashboardAnalytics()
        {
            var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
            return Ok(analytics);
        }

        [HttpGet("revenue")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<MonthlyRevenueDto>>> GetMonthlyRevenue([FromQuery] int months = 12)
        {
            var data = await _analyticsService.GetMonthlyRevenueAsync(months);
            return Ok(data);
        }

        [HttpGet("orders")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<MonthlyOrdersDto>>> GetMonthlyOrders([FromQuery] int months = 12)
        {
            var data = await _analyticsService.GetMonthlyOrdersAsync(months);
            return Ok(data);
        }

        [HttpGet("top-customers")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<TopCustomerDto>>> GetTopCustomers([FromQuery] int count = 10)
        {
            var data = await _analyticsService.GetTopCustomersAsync(count);
            return Ok(data);
        }

        [HttpGet("executors")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<ExecutorPerformanceDto>>> GetExecutorPerformance()
        {
            var data = await _analyticsService.GetExecutorPerformanceAsync();
            return Ok(data);
        }

        [HttpGet("services")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<List<PopularServiceDto>>> GetPopularServices([FromQuery] int count = 10)
        {
            var data = await _analyticsService.GetPopularServicesAsync(count);
            return Ok(data);
        }
    }
}
