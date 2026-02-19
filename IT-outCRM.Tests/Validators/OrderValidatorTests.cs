using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Validators.Order;

namespace IT_outCRM.Tests.Validators;

public class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _validator = new();

    [Fact]
    public void Valid_Order_PassesValidation()
    {
        var dto = new CreateOrderDto { Name = "Test Order", Price = 100 };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Name_FailsValidation()
    {
        var dto = new CreateOrderDto { Name = "", Price = 100 };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Long_Name_FailsValidation()
    {
        var dto = new CreateOrderDto { Name = new string('a', 201), Price = 100 };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Zero_Price_FailsValidation()
    {
        var dto = new CreateOrderDto { Name = "Order", Price = 0 };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Negative_Price_FailsValidation()
    {
        var dto = new CreateOrderDto { Name = "Order", Price = -1 };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Long_Description_FailsValidation()
    {
        var dto = new CreateOrderDto { Name = "Order", Price = 100, Description = new string('a', 1001) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Description);
    }
}

public class UpdateOrderValidatorTests
{
    private readonly UpdateOrderValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateOrderDto
        {
            Id = Guid.NewGuid(),
            Name = "Order",
            Price = 100,
            CustomerId = Guid.NewGuid(),
            OrderStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateOrderDto
        {
            Id = Guid.Empty,
            Name = "Order",
            Price = 100,
            CustomerId = Guid.NewGuid(),
            OrderStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_CustomerId_FailsValidation()
    {
        var dto = new UpdateOrderDto
        {
            Id = Guid.NewGuid(),
            Name = "Order",
            Price = 100,
            CustomerId = Guid.Empty,
            OrderStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Fact]
    public void Empty_OrderStatusId_FailsValidation()
    {
        var dto = new UpdateOrderDto
        {
            Id = Guid.NewGuid(),
            Name = "Order",
            Price = 100,
            CustomerId = Guid.NewGuid(),
            OrderStatusId = Guid.Empty
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.OrderStatusId);
    }
}
