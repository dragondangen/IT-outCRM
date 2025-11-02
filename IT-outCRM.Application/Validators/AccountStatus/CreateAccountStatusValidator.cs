using FluentValidation;
using IT_outCRM.Application.DTOs.AccountStatus;

namespace IT_outCRM.Application.Validators.AccountStatus
{
    public class CreateAccountStatusValidator : AbstractValidator<CreateAccountStatusDto>
    {
        public CreateAccountStatusValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название статуса обязательно")
                .MaximumLength(100).WithMessage("Название статуса не должно превышать 100 символов");
        }
    }
}

