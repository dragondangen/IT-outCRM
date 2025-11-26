using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Blazor.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название заказа обязательно")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public Guid? ExecutorId { get; set; }
        public string ExecutorName { get; set; } = string.Empty;

        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; } = string.Empty;

        public Guid? SupportTeamId { get; set; }
    }

    public class CreateOrderModel
    {
        [Required(ErrorMessage = "Название заказа обязательно")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        // Made nullable to allow frontend logic to handle defaults/roles
        public Guid? CustomerId { get; set; }

        public Guid? ExecutorId { get; set; }

        public Guid? OrderStatusId { get; set; }

        // SupportTeamId необязателен - команда поддержки может быть назначена позже
        public Guid? SupportTeamId { get; set; }
    }
}
