using FluentValidation;
using IT_outCRM.Application.DTOs.Account;

namespace IT_outCRM.Application.Validators.Account
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Название компании обязательно")
                .MaximumLength(200).WithMessage("Название компании не должно превышать 200 символов");

            RuleFor(x => x.FoundingDate)
                .NotEmpty().WithMessage("Дата основания обязательна")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Дата основания не может быть в будущем");

            RuleFor(x => x.AccountStatusId)
                .NotEmpty().WithMessage("Статус аккаунта обязателен");
        }
    }
}

