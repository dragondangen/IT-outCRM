using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Blazor.Models
{
    public class CustomerModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Аккаунт обязателен")]
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Компания обязательна")]
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    public class CreateCustomerModel
    {
        [Required(ErrorMessage = "Аккаунт обязателен")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "Компания обязательна")]
        public Guid CompanyId { get; set; }
    }
}

