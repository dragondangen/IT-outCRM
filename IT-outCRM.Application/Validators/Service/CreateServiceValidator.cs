using FluentValidation;
using IT_outCRM.Application.DTOs.Service;

namespace IT_outCRM.Application.Validators.Service
{
    public class CreateServiceValidator : AbstractValidator<CreateServiceDto>
    {
        public CreateServiceValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название услуги обязательно")
                .MinimumLength(3).WithMessage("Название должно быть не менее 3 символов")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Цена должна быть положительным числом");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Длительность должна быть положительным числом");

            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Категория не должна превышать 100 символов");

            RuleFor(x => x.ExecutorId)
                .NotEmpty().WithMessage("Исполнитель обязателен");
        }
    }
}

