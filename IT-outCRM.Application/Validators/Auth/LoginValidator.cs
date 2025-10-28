using FluentValidation;
using IT_outCRM.Application.DTOs.Auth;

namespace IT_outCRM.Application.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .MinimumLength(3).WithMessage("Имя пользователя должно содержать минимум 3 символа");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");
        }
    }
}

