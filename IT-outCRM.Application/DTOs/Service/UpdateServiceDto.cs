using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Application.DTOs.Service
{
    public class UpdateServiceDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название услуги обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Длительность обязательна")]
        [Range(1, int.MaxValue, ErrorMessage = "Длительность должна быть положительным числом")]
        public int Duration { get; set; } = 1;

        [StringLength(100, ErrorMessage = "Категория не должна превышать 100 символов")]
        public string Category { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Исполнитель обязателен")]
        public Guid ExecutorId { get; set; }
    }
}

