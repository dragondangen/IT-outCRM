using FluentValidation;
using IT_outCRM.Application.DTOs.Order;

namespace IT_outCRM.Application.Validators.Order
{
    public class UpdateOrderValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID обязателен");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название заказа обязательно")
                .MaximumLength(200).WithMessage("Название заказа не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");

            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("ID клиента обязателен");

            RuleFor(x => x.OrderStatusId)
                .NotEmpty().WithMessage("ID статуса заказа обязателен");

            // ExecutorId и SupportTeamId необязательны - могут быть назначены позже
        }
    }
}

