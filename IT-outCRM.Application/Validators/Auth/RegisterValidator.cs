using FluentValidation;
using IT_outCRM.Application.DTOs.Auth;

namespace IT_outCRM.Application.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .MinimumLength(3).WithMessage("Имя пользователя должно содержать минимум 3 символа")
                .MaximumLength(50).WithMessage("Имя пользователя не должно превышать 50 символов")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и _");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
                .Matches(@"[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches(@"[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches(@"[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");

            RuleFor(x => x.Role)
                .Must(role => role == "User" || role == "Admin" || role == "Manager")
                .WithMessage("Роль должна быть: User, Admin или Manager");
        }
    }
}

