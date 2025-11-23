using System.ComponentModel.DataAnnotations;

namespace IT_outCRM.Blazor.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

