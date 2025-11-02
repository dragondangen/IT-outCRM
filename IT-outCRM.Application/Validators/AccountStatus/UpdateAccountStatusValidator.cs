using FluentValidation;
using IT_outCRM.Application.DTOs.AccountStatus;

namespace IT_outCRM.Application.Validators.AccountStatus
{
    public class UpdateAccountStatusValidator : AbstractValidator<UpdateAccountStatusDto>
    {
        public UpdateAccountStatusValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id обязателен");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название статуса обязательно")
                .MaximumLength(100).WithMessage("Название статуса не должно превышать 100 символов");
        }
    }
}

