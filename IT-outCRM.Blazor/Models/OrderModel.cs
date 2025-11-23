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

        [Required(ErrorMessage = "Клиент обязателен")]
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Исполнитель обязателен")]
        public Guid ExecutorId { get; set; }
        public string ExecutorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Статус обязателен")]
        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; } = string.Empty;

        public Guid SupportTeamId { get; set; }
    }

    public class CreateOrderModel
    {
        [Required(ErrorMessage = "Название заказа обязательно")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Клиент обязателен")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Исполнитель обязателен")]
        public Guid ExecutorId { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public Guid OrderStatusId { get; set; }

        public Guid SupportTeamId { get; set; }
    }
}

