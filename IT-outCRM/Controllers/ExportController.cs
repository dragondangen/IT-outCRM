using System.Text;
using IT_outCRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Manager")]
    public class ExportController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public ExportController(IUnitOfWork unitOfWork, ILogger<ExportController> logger) : base(logger)
        {
            _uow = unitOfWork;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> ExportOrders()
        {
            var orders = (await _uow.Orders.GetAllAsync()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ID;Название;Описание;Цена;Клиент;Исполнитель;Статус");

            foreach (var o in orders)
            {
                sb.AppendLine(string.Join(";",
                    Escape(o.Id.ToString()),
                    Escape(o.Name),
                    Escape(o.Description),
                    o.Price.ToString("F2"),
                    Escape(o.Customer?.Account?.CompanyName),
                    Escape(o.Executor?.Account?.CompanyName),
                    Escape(o.OrderStatus?.Name)));
            }

            return CsvFile(sb, "orders.csv");
        }

        [HttpGet("deals")]
        public async Task<IActionResult> ExportDeals()
        {
            var deals = (await _uow.Deals.GetAllAsync()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ID;Статус;Цена;Дедлайн;Условия;Рейтинг заказчика;Рейтинг исполнителя;Создана;Обновлена");

            foreach (var d in deals)
            {
                sb.AppendLine(string.Join(";",
                    Escape(d.Id.ToString()),
                    Escape(d.Status),
                    d.AgreedPrice.ToString("F2"),
                    d.Deadline?.ToString("yyyy-MM-dd") ?? "",
                    Escape(d.Terms),
                    d.CustomerRating?.ToString() ?? "",
                    d.ExecutorRating?.ToString() ?? "",
                    d.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    d.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? ""));
            }

            return CsvFile(sb, "deals.csv");
        }

        [HttpGet("customers")]
        public async Task<IActionResult> ExportCustomers()
        {
            var customers = (await _uow.Customers.GetAllAsync()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ID;Аккаунт;Компания");

            foreach (var c in customers)
            {
                sb.AppendLine(string.Join(";",
                    Escape(c.Id.ToString()),
                    Escape(c.Account?.CompanyName),
                    Escape(c.Company?.Name)));
            }

            return CsvFile(sb, "customers.csv");
        }

        [HttpGet("executors")]
        public async Task<IActionResult> ExportExecutors()
        {
            var executors = (await _uow.Executors.GetAllAsync()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ID;Аккаунт;Компания;Завершено заказов");

            foreach (var e in executors)
            {
                sb.AppendLine(string.Join(";",
                    Escape(e.Id.ToString()),
                    Escape(e.Account?.CompanyName),
                    Escape(e.Company?.Name),
                    e.CompletedOrders.ToString()));
            }

            return CsvFile(sb, "executors.csv");
        }

        [HttpGet("services")]
        public async Task<IActionResult> ExportServices()
        {
            var services = (await _uow.Services.GetAllAsync()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ID;Название;Категория;Описание;Цена;Активна");

            foreach (var s in services)
            {
                sb.AppendLine(string.Join(";",
                    Escape(s.Id.ToString()),
                    Escape(s.Name),
                    Escape(s.Category),
                    Escape(s.Description),
                    s.Price.ToString("F2"),
                    s.IsActive ? "Да" : "Нет"));
            }

            return CsvFile(sb, "services.csv");
        }

        private FileContentResult CsvFile(StringBuilder sb, string fileName)
        {
            var bom = Encoding.UTF8.GetPreamble();
            var content = Encoding.UTF8.GetBytes(sb.ToString());
            var result = new byte[bom.Length + content.Length];
            bom.CopyTo(result, 0);
            content.CopyTo(result, bom.Length);
            return File(result, "text/csv; charset=utf-8", fileName);
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            value = value.Replace("\"", "\"\"");
            if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value}\"";
            return value;
        }
    }
}
