namespace IT_outCRM.Application.DTOs.Analytics
{
    public class DashboardAnalyticsDto
    {
        public OverviewDto Overview { get; set; } = new();
        public RevenueAnalyticsDto Revenue { get; set; } = new();
        public List<OrdersByStatusDto> OrdersByStatus { get; set; } = new();
        public List<DealsByStatusDto> DealsByStatus { get; set; } = new();
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
        public List<ExecutorPerformanceDto> ExecutorPerformance { get; set; } = new();
        public List<PopularServiceDto> PopularServices { get; set; } = new();
        public List<MonthlyOrdersDto> MonthlyOrders { get; set; } = new();
        public ConversionFunnelDto ConversionFunnel { get; set; } = new();
    }

    public class OverviewDto
    {
        public int TotalOrders { get; set; }
        public int ActiveOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalDeals { get; set; }
        public int ActiveDeals { get; set; }
        public int CompletedDeals { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalExecutors { get; set; }
        public int TotalServices { get; set; }
        public int TotalAccounts { get; set; }
        public int NewAccountsThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal AverageDealValue { get; set; }
        public double AverageExecutorRating { get; set; }
    }

    public class RevenueAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueLastMonth { get; set; }
        public decimal RevenueGrowthPercent { get; set; }
        public decimal RevenueFromOrders { get; set; }
        public decimal RevenueFromDeals { get; set; }
    }

    public class OrdersByStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class DealsByStatusDto
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal OrdersRevenue { get; set; }
        public decimal DealsRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyOrdersDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Created { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    public class TopCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public int DealCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class ExecutorPerformanceDto
    {
        public Guid ExecutorId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int CompletedOrders { get; set; }
        public int ActiveOrders { get; set; }
        public int ServiceCount { get; set; }
        public decimal TotalEarned { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class PopularServiceDto
    {
        public Guid ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExecutorName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ConversionFunnelDto
    {
        public int PublishedOrders { get; set; }
        public int OrdersInProgress { get; set; }
        public int OrdersOnReview { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalDeals { get; set; }
        public int AgreedDeals { get; set; }
        public int CompletedDeals { get; set; }
        public decimal OrderCompletionRate { get; set; }
        public decimal DealConversionRate { get; set; }
    }
}
