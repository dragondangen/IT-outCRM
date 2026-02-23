using IT_outCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public SearchController(IUnitOfWork unitOfWork, ILogger<SearchController> logger) : base(logger)
        {
            _uow = unitOfWork;
        }

        /// <summary>
        /// Глобальный поиск по всем сущностям CRM
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new GlobalSearchResult());

            limit = Math.Clamp(limit, 1, 50);
            var query = q.Trim();

            var results = new GlobalSearchResult();

            var customersTask = Task.Run(async () =>
            {
                var all = await _uow.Customers.GetAllAsync();
                return all.Where(c =>
                    Contains(c.Account?.CompanyName, query) ||
                    Contains(c.Company?.Name, query))
                    .Take(limit)
                    .Select(c => new SearchHit("customer", c.Id, c.Account?.CompanyName ?? "—", $"Компания: {c.Company?.Name ?? "—"}"))
                    .ToList();
            });

            var executorsTask = Task.Run(async () =>
            {
                var all = await _uow.Executors.GetAllAsync();
                return all.Where(e =>
                    Contains(e.Account?.CompanyName, query) ||
                    Contains(e.Company?.Name, query))
                    .Take(limit)
                    .Select(e => new SearchHit("executor", e.Id, e.Account?.CompanyName ?? "—", $"Завершено заказов: {e.CompletedOrders}"))
                    .ToList();
            });

            var ordersTask = Task.Run(async () =>
            {
                var all = await _uow.Orders.GetAllAsync();
                return all.Where(o =>
                    Contains(o.Name, query) ||
                    Contains(o.Description, query))
                    .Take(limit)
                    .Select(o => new SearchHit("order", o.Id, o.Name, $"{o.Price:N0} ₽ — {o.OrderStatus?.Name ?? "—"}"))
                    .ToList();
            });

            var companiesTask = Task.Run(async () =>
            {
                var all = await _uow.Companies.GetAllAsync();
                return all.Where(c =>
                    Contains(c.Name, query) ||
                    Contains(c.Inn, query))
                    .Take(limit)
                    .Select(c => new SearchHit("company", c.Id, c.Name, $"ИНН: {c.Inn}"))
                    .ToList();
            });

            var servicesTask = Task.Run(async () =>
            {
                var all = await _uow.Services.GetAllAsync();
                return all.Where(s =>
                    Contains(s.Name, query) ||
                    Contains(s.Category, query) ||
                    Contains(s.Description, query))
                    .Take(limit)
                    .Select(s => new SearchHit("service", s.Id, s.Name, $"{s.Category} — {s.Price:N0} ₽"))
                    .ToList();
            });

            var dealsTask = Task.Run(async () =>
            {
                var all = await _uow.Deals.GetAllAsync();
                return all.Where(d =>
                    Contains(d.Terms, query) ||
                    Contains(d.Status, query))
                    .Take(limit)
                    .Select(d => new SearchHit("deal", d.Id, $"Сделка #{d.Id.ToString()[..8]}", $"{d.Status} — {d.AgreedPrice:N0} ₽"))
                    .ToList();
            });

            await Task.WhenAll(customersTask, executorsTask, ordersTask, companiesTask, servicesTask, dealsTask);

            results.Customers = customersTask.Result;
            results.Executors = executorsTask.Result;
            results.Orders = ordersTask.Result;
            results.Companies = companiesTask.Result;
            results.Services = servicesTask.Result;
            results.Deals = dealsTask.Result;
            results.TotalCount = results.Customers.Count + results.Executors.Count +
                                  results.Orders.Count + results.Companies.Count +
                                  results.Services.Count + results.Deals.Count;

            return Ok(results);
        }

        private static bool Contains(string? value, string query) =>
            !string.IsNullOrEmpty(value) && value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    public class GlobalSearchResult
    {
        public int TotalCount { get; set; }
        public List<SearchHit> Customers { get; set; } = new();
        public List<SearchHit> Executors { get; set; } = new();
        public List<SearchHit> Orders { get; set; } = new();
        public List<SearchHit> Companies { get; set; } = new();
        public List<SearchHit> Services { get; set; } = new();
        public List<SearchHit> Deals { get; set; } = new();
    }

    public record SearchHit(string Type, Guid Id, string Title, string Subtitle);
}
