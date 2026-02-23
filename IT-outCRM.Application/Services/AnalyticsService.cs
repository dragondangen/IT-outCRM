using IT_outCRM.Application.DTOs.Analytics;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;
using System.Globalization;

namespace IT_outCRM.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _uow;
        private static readonly CultureInfo RuCulture = new("ru-RU");

        private static readonly Dictionary<string, string> OrderStatusColors = new()
        {
            ["Опубликован"] = "#3498db",
            ["В работе"] = "#f39c12",
            ["На проверке"] = "#9b59b6",
            ["Завершён"] = "#27ae60",
            ["Отменён"] = "#e74c3c"
        };

        private static readonly Dictionary<string, string> DealStatusColors = new()
        {
            ["Новая"] = "#3498db",
            ["Предложена"] = "#2ecc71",
            ["Согласована"] = "#1abc9c",
            ["В работе"] = "#f39c12",
            ["На проверке"] = "#9b59b6",
            ["Завершена"] = "#27ae60",
            ["Отменена"] = "#e74c3c"
        };

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        /// <summary>
        /// Pre-loaded snapshot of all data needed for analytics.
        /// Loaded once per request to avoid multiple DB round-trips.
        /// </summary>
        private sealed class DataSnapshot
        {
            public List<Order> Orders = new();
            public List<Deal> Deals = new();
            public List<Customer> Customers = new();
            public List<Executor> Executors = new();
            public List<Service> Services = new();
            public List<Account> Accounts = new();
            public List<OrderStatus> OrderStatuses = new();

            public Guid? CompletedStatusId;
            public Guid? CancelledStatusId;
            public Guid? InProgressStatusId;
            public Guid? OnReviewStatusId;
            public Guid? PublishedStatusId;
            public HashSet<Guid> ActiveStatusIds = new();
        }

        private async Task<DataSnapshot> LoadSnapshotAsync()
        {
            var snap = new DataSnapshot
            {
                Orders = (await _uow.Orders.GetAllAsync()).ToList(),
                Deals = (await _uow.Deals.GetAllAsync()).ToList(),
                Customers = (await _uow.Customers.GetAllAsync()).ToList(),
                Executors = (await _uow.Executors.GetAllAsync()).ToList(),
                Services = (await _uow.Services.GetAllAsync()).ToList(),
                Accounts = (await _uow.Accounts.GetAllAsync()).ToList(),
                OrderStatuses = (await _uow.OrderStatuses.GetAllAsync()).ToList()
            };

            snap.CompletedStatusId = snap.OrderStatuses.FirstOrDefault(s => s.Name == "Завершён")?.Id;
            snap.CancelledStatusId = snap.OrderStatuses.FirstOrDefault(s => s.Name == "Отменён")?.Id;
            snap.InProgressStatusId = snap.OrderStatuses.FirstOrDefault(s => s.Name == "В работе")?.Id;
            snap.OnReviewStatusId = snap.OrderStatuses.FirstOrDefault(s => s.Name == "На проверке")?.Id;
            snap.PublishedStatusId = snap.OrderStatuses.FirstOrDefault(s => s.Name == "Опубликован")?.Id;
            snap.ActiveStatusIds = snap.OrderStatuses
                .Where(s => s.Name != "Завершён" && s.Name != "Отменён")
                .Select(s => s.Id).ToHashSet();

            return snap;
        }

        public async Task<DashboardAnalyticsDto> GetDashboardAnalyticsAsync()
        {
            var s = await LoadSnapshotAsync();

            var completedOrders = s.Orders.Where(o => o.OrderStatusId == s.CompletedStatusId).ToList();
            var cancelledOrders = s.Orders.Where(o => o.OrderStatusId == s.CancelledStatusId).ToList();
            var activeOrders = s.Orders.Where(o =>
                o.OrderStatusId != s.CompletedStatusId && o.OrderStatusId != s.CancelledStatusId).ToList();

            var completedDeals = s.Deals.Where(d => d.Status == "Завершена").ToList();
            var activeDeals = s.Deals.Where(d =>
                d.Status != "Завершена" && d.Status != "Отменена").ToList();

            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastMonthStart = thisMonthStart.AddMonths(-1);

            var revenueFromCompletedOrders = completedOrders.Sum(o => o.Price);
            var revenueFromCompletedDeals = completedDeals.Sum(d => d.AgreedPrice);
            var totalRevenue = revenueFromCompletedOrders + revenueFromCompletedDeals;

            var dealsThisMonth = completedDeals.Where(d => d.UpdatedAt >= thisMonthStart).ToList();
            var revenueThisMonth = dealsThisMonth.Sum(d => d.AgreedPrice);

            var dealsLastMonth = completedDeals.Where(d =>
                d.UpdatedAt >= lastMonthStart && d.UpdatedAt < thisMonthStart).ToList();
            var revenueLastMonth = dealsLastMonth.Sum(d => d.AgreedPrice);

            var executorRatings = s.Deals
                .Where(d => d.ExecutorRating.HasValue)
                .Select(d => d.ExecutorRating!.Value)
                .ToList();
            var avgRating = executorRatings.Count > 0 ? executorRatings.Average() : 0;

            return new DashboardAnalyticsDto
            {
                Overview = new OverviewDto
                {
                    TotalOrders = s.Orders.Count,
                    ActiveOrders = activeOrders.Count,
                    CompletedOrders = completedOrders.Count,
                    CancelledOrders = cancelledOrders.Count,
                    TotalDeals = s.Deals.Count,
                    ActiveDeals = activeDeals.Count,
                    CompletedDeals = completedDeals.Count,
                    TotalCustomers = s.Customers.Count,
                    TotalExecutors = s.Executors.Count,
                    TotalServices = s.Services.Count(svc => svc.IsActive),
                    TotalAccounts = s.Accounts.Count,
                    NewAccountsThisMonth = s.Accounts.Count(a => a.FoundingDate >= thisMonthStart),
                    TotalRevenue = totalRevenue,
                    RevenueThisMonth = revenueThisMonth,
                    AverageOrderValue = s.Orders.Count > 0 ? s.Orders.Average(o => o.Price) : 0,
                    AverageDealValue = s.Deals.Count > 0
                        ? s.Deals.Where(d => d.AgreedPrice > 0).DefaultIfEmpty().Average(d => d?.AgreedPrice ?? 0)
                        : 0,
                    AverageExecutorRating = Math.Round(avgRating, 1)
                },
                Revenue = new RevenueAnalyticsDto
                {
                    TotalRevenue = totalRevenue,
                    RevenueThisMonth = revenueThisMonth,
                    RevenueLastMonth = revenueLastMonth,
                    RevenueGrowthPercent = revenueLastMonth > 0
                        ? Math.Round((revenueThisMonth - revenueLastMonth) / revenueLastMonth * 100, 1)
                        : 0,
                    RevenueFromOrders = revenueFromCompletedOrders,
                    RevenueFromDeals = revenueFromCompletedDeals
                },
                OrdersByStatus = s.OrderStatuses.Select(os =>
                {
                    var count = s.Orders.Count(o => o.OrderStatusId == os.Id);
                    return new OrdersByStatusDto
                    {
                        StatusName = os.Name,
                        Count = count,
                        Percentage = s.Orders.Count > 0 ? Math.Round((decimal)count / s.Orders.Count * 100, 1) : 0,
                        Color = OrderStatusColors.GetValueOrDefault(os.Name, "#95a5a6")
                    };
                }).Where(x => x.Count > 0).OrderByDescending(x => x.Count).ToList(),

                DealsByStatus = s.Deals.GroupBy(d => d.Status).Select(g => new DealsByStatusDto
                {
                    StatusName = g.Key,
                    Count = g.Count(),
                    Percentage = s.Deals.Count > 0 ? Math.Round((decimal)g.Count() / s.Deals.Count * 100, 1) : 0,
                    Color = DealStatusColors.GetValueOrDefault(g.Key, "#95a5a6")
                }).OrderByDescending(x => x.Count).ToList(),

                MonthlyRevenue = BuildMonthlyRevenue(s, 12),
                MonthlyOrders = BuildMonthlyOrders(s, 12),
                TopCustomers = BuildTopCustomers(s, 10),
                ExecutorPerformance = BuildExecutorPerformance(s),
                PopularServices = BuildPopularServices(s, 10),

                ConversionFunnel = new ConversionFunnelDto
                {
                    PublishedOrders = s.PublishedStatusId.HasValue ? s.Orders.Count(o => o.OrderStatusId == s.PublishedStatusId) : 0,
                    OrdersInProgress = s.InProgressStatusId.HasValue ? s.Orders.Count(o => o.OrderStatusId == s.InProgressStatusId) : 0,
                    OrdersOnReview = s.OnReviewStatusId.HasValue ? s.Orders.Count(o => o.OrderStatusId == s.OnReviewStatusId) : 0,
                    CompletedOrders = completedOrders.Count,
                    TotalDeals = s.Deals.Count,
                    AgreedDeals = s.Deals.Count(d => d.Status is "Согласована" or "В работе" or "На проверке" or "Завершена"),
                    CompletedDeals = completedDeals.Count,
                    OrderCompletionRate = s.Orders.Count > 0 ? Math.Round((decimal)completedOrders.Count / s.Orders.Count * 100, 1) : 0,
                    DealConversionRate = s.Deals.Count > 0 ? Math.Round((decimal)completedDeals.Count / s.Deals.Count * 100, 1) : 0
                }
            };
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 12)
        {
            var s = await LoadSnapshotAsync();
            return BuildMonthlyRevenue(s, months);
        }

        public async Task<List<MonthlyOrdersDto>> GetMonthlyOrdersAsync(int months = 12)
        {
            var s = await LoadSnapshotAsync();
            return BuildMonthlyOrders(s, months);
        }

        public async Task<List<TopCustomerDto>> GetTopCustomersAsync(int count = 10)
        {
            var s = await LoadSnapshotAsync();
            return BuildTopCustomers(s, count);
        }

        public async Task<List<ExecutorPerformanceDto>> GetExecutorPerformanceAsync()
        {
            var s = await LoadSnapshotAsync();
            return BuildExecutorPerformance(s);
        }

        public async Task<List<PopularServiceDto>> GetPopularServicesAsync(int count = 10)
        {
            var s = await LoadSnapshotAsync();
            return BuildPopularServices(s, count);
        }

        private static List<MonthlyRevenueDto> BuildMonthlyRevenue(DataSnapshot s, int months)
        {
            var completedDeals = s.Deals.Where(d => d.Status == "Завершена").ToList();
            var completedOrders = s.Orders.Where(o => o.OrderStatusId == s.CompletedStatusId).ToList();
            var result = new List<MonthlyRevenueDto>(months);
            var now = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthEnd = monthStart.AddMonths(1);

                var dealsRevenue = completedDeals
                    .Where(d => d.UpdatedAt >= monthStart && d.UpdatedAt < monthEnd)
                    .Sum(d => d.AgreedPrice);

                var monthName = RuCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

                result.Add(new MonthlyRevenueDto
                {
                    Month = char.ToUpper(monthName[0]) + monthName[1..],
                    Year = date.Year,
                    OrdersRevenue = 0,
                    DealsRevenue = dealsRevenue,
                    TotalRevenue = dealsRevenue
                });
            }
            return result;
        }

        private static List<MonthlyOrdersDto> BuildMonthlyOrders(DataSnapshot s, int months)
        {
            var result = new List<MonthlyOrdersDto>(months);
            var now = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                var monthStart = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthEnd = monthStart.AddMonths(1);

                var created = s.Deals.Count(d => d.CreatedAt >= monthStart && d.CreatedAt < monthEnd);
                var completed = s.Deals.Count(d => d.Status == "Завершена" && d.UpdatedAt >= monthStart && d.UpdatedAt < monthEnd);
                var cancelled = s.Deals.Count(d => d.Status == "Отменена" && d.UpdatedAt >= monthStart && d.UpdatedAt < monthEnd);

                var monthName = RuCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);

                result.Add(new MonthlyOrdersDto
                {
                    Month = char.ToUpper(monthName[0]) + monthName[1..],
                    Year = date.Year,
                    Created = created,
                    Completed = completed,
                    Cancelled = cancelled
                });
            }
            return result;
        }

        private static List<TopCustomerDto> BuildTopCustomers(DataSnapshot s, int count)
        {
            var accountLookup = s.Accounts.ToDictionary(a => a.Id);
            var ordersByCustomer = s.Orders.GroupBy(o => o.CustomerId).ToDictionary(g => g.Key, g => g.ToList());
            var dealsByCustomer = s.Deals.GroupBy(d => d.CustomerId).ToDictionary(g => g.Key, g => g.ToList());

            return s.Customers.Select(c =>
            {
                var customerOrders = ordersByCustomer.GetValueOrDefault(c.Id) ?? new List<Order>();
                var customerDeals = dealsByCustomer.GetValueOrDefault(c.Id) ?? new List<Deal>();
                accountLookup.TryGetValue(c.AccountId, out var account);

                return new TopCustomerDto
                {
                    CustomerId = c.Id,
                    CompanyName = account?.CompanyName ?? "—",
                    OrderCount = customerOrders.Count,
                    DealCount = customerDeals.Count,
                    TotalSpent = customerOrders.Sum(o => o.Price) +
                                 customerDeals.Where(d => d.Status == "Завершена").Sum(d => d.AgreedPrice)
                };
            }).OrderByDescending(c => c.TotalSpent).Take(count).ToList();
        }

        private static List<ExecutorPerformanceDto> BuildExecutorPerformance(DataSnapshot s)
        {
            var accountLookup = s.Accounts.ToDictionary(a => a.Id);
            var ordersByExecutor = s.Orders.Where(o => o.ExecutorId.HasValue)
                .GroupBy(o => o.ExecutorId!.Value).ToDictionary(g => g.Key, g => g.ToList());
            var dealsByExecutor = s.Deals.GroupBy(d => d.ExecutorId).ToDictionary(g => g.Key, g => g.ToList());
            var servicesByExecutor = s.Services.Where(svc => svc.IsActive)
                .GroupBy(svc => svc.ExecutorId).ToDictionary(g => g.Key, g => g.ToList());

            return s.Executors.Select(e =>
            {
                var executorOrders = ordersByExecutor.GetValueOrDefault(e.Id) ?? new List<Order>();
                var executorDeals = dealsByExecutor.GetValueOrDefault(e.Id) ?? new List<Deal>();
                var executorServices = servicesByExecutor.GetValueOrDefault(e.Id) ?? new List<Service>();
                accountLookup.TryGetValue(e.AccountId, out var account);
                var ratings = executorDeals.Where(d => d.ExecutorRating.HasValue).Select(d => d.ExecutorRating!.Value).ToList();

                return new ExecutorPerformanceDto
                {
                    ExecutorId = e.Id,
                    CompanyName = account?.CompanyName ?? "—",
                    CompletedOrders = e.CompletedOrders,
                    ActiveOrders = executorOrders.Count(o => s.ActiveStatusIds.Contains(o.OrderStatusId)),
                    ServiceCount = executorServices.Count,
                    TotalEarned = executorOrders.Where(o => o.OrderStatusId == s.CompletedStatusId).Sum(o => o.Price)
                        + executorDeals.Where(d => d.Status == "Завершена").Sum(d => d.AgreedPrice),
                    AverageRating = ratings.Count > 0 ? Math.Round(ratings.Average(), 1) : 0,
                    ReviewCount = ratings.Count
                };
            }).OrderByDescending(e => e.TotalEarned).ToList();
        }

        private static List<PopularServiceDto> BuildPopularServices(DataSnapshot s, int count)
        {
            var accountLookup = s.Accounts.ToDictionary(a => a.Id);
            var executorLookup = s.Executors.ToDictionary(e => e.Id);
            var dealsByService = s.Deals.GroupBy(d => d.ServiceId).ToDictionary(g => g.Key, g => g.ToList());

            return s.Services.Where(svc => svc.IsActive).Select(svc =>
            {
                var serviceDeals = dealsByService.GetValueOrDefault(svc.Id) ?? new List<Deal>();
                executorLookup.TryGetValue(svc.ExecutorId, out var executor);
                var executorAccount = executor != null && accountLookup.TryGetValue(executor.AccountId, out var a) ? a : null;

                return new PopularServiceDto
                {
                    ServiceId = svc.Id,
                    Name = svc.Name,
                    Category = svc.Category,
                    ExecutorName = executorAccount?.CompanyName ?? "—",
                    Price = svc.Price,
                    OrderCount = serviceDeals.Count,
                    TotalRevenue = serviceDeals.Where(d => d.Status == "Завершена").Sum(d => d.AgreedPrice)
                };
            }).OrderByDescending(x => x.OrderCount).Take(count).ToList();
        }
    }
}
