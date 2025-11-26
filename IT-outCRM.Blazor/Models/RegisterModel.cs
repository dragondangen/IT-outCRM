using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Blazor.Models
{
    /// <summary>
    /// Модель для UI с валидацией
    /// </summary>
    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email адрес")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 128 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).{8,}$", 
            ErrorMessage = "Пароль должен содержать: заглавную букву, строчную букву, цифру и специальный символ")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        // Extended Registration Fields
        [Required(ErrorMessage = "Название компании обязательно")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ИНН обязателен")]
        public string Inn { get; set; } = string.Empty;

        public string LegalForm { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите тип пользователя")]
        public string UserType { get; set; } = "Customer"; // Customer or Executor
    }

    /// <summary>
    /// DTO для отправки на backend (соответствует RegisterDto)
    /// </summary>
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
