using FluentValidation;
using IT_outCRM.Application.DTOs.Order;

namespace IT_outCRM.Application.Validators.Order
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название заказа обязательно")
                .MaximumLength(200).WithMessage("Название заказа не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");

            // CustomerId, OrderStatusId, and SupportTeamId могут быть пустыми, 
            // если они будут автоматически заполнены контроллером для клиентов.
            // Контроллер проверит и заполнит эти поля перед созданием заказа.
            // ExecutorId необязателен (может быть null для опубликованных заказов)
        }
    }
}

