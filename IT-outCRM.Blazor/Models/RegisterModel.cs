using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Blazor.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "От 3 до 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_\-\.]+$", ErrorMessage = "Только латиница, цифры, _ - .")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "От 8 до 128 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        [Required(ErrorMessage = "Название компании обязательно")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "От 2 до 200 символов")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ИНН обязателен")]
        [RegularExpression(@"^\d{10}(\d{2})?$", ErrorMessage = "ИНН: 10 или 12 цифр")]
        public string Inn { get; set; } = string.Empty;

        [StringLength(50)]
        public string LegalForm { get; set; } = string.Empty;

        [RegularExpression(@"^[\+]?[\d\s\-\(\)]{7,18}$|^$",
            ErrorMessage = "Введите корректный номер телефона")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите тип пользователя")]
        public string UserType { get; set; } = "Customer";
    }

    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

        public string CompanyName { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string LegalForm { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }
}
