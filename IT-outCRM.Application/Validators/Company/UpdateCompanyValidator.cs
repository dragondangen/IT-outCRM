using FluentValidation;
using IT_outCRM.Application.DTOs.Company;

namespace IT_outCRM.Application.Validators.Company
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyDto>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID обязателен");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название компании обязательно")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.Inn)
                .NotEmpty().WithMessage("ИНН обязателен")
                .Matches(@"^\d{10}$|^\d{12}$").WithMessage("ИНН должен содержать 10 или 12 цифр");

            RuleFor(x => x.LegalForm)
                .NotEmpty().WithMessage("Юридическая форма обязательна");

            RuleFor(x => x.ContactPersonId)
                .NotEmpty().WithMessage("ID контактного лица обязателен");
        }
    }
}

