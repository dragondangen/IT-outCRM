using FluentValidation;
using IT_outCRM.Application.DTOs.ContactPerson;

namespace IT_outCRM.Application.Validators.ContactPerson
{
    public class CreateContactPersonValidator : AbstractValidator<CreateContactPersonDto>
    {
        public CreateContactPersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов");

            RuleFor(x => x.MiddleName)
                .MaximumLength(100).WithMessage("Отчество не должно превышать 100 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен")
                .Matches(@"^(\+7|8)?[\s\-]?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}$")
                .WithMessage("Некорректный формат номера телефона");
        }
    }
}

