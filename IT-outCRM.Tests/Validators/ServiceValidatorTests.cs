using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Service;
using IT_outCRM.Application.Validators.Service;

namespace IT_outCRM.Tests.Validators;

public class CreateServiceValidatorTests
{
    private readonly CreateServiceValidator _validator = new();

    private CreateServiceDto ValidDto() => new()
    {
        Name = "Web Development",
        Description = "Full-stack development",
        Price = 5000,
        Duration = 30,
        Category = "IT",
        IsActive = true,
        ExecutorId = Guid.NewGuid()
    };

    [Fact]
    public void Valid_Service_PassesValidation()
    {
        _validator.TestValidate(ValidDto()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Name_FailsValidation()
    {
        var dto = ValidDto();
        dto.Name = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Short_Name_FailsValidation()
    {
        var dto = ValidDto();
        dto.Name = "AB";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Long_Name_FailsValidation()
    {
        var dto = ValidDto();
        dto.Name = new string('a', 201);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Long_Description_FailsValidation()
    {
        var dto = ValidDto();
        dto.Description = new string('a', 2001);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Negative_Price_FailsValidation()
    {
        var dto = ValidDto();
        dto.Price = -1;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Zero_Price_PassesValidation()
    {
        var dto = ValidDto();
        dto.Price = 0;
        _validator.TestValidate(dto).ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Zero_Duration_FailsValidation()
    {
        var dto = ValidDto();
        dto.Duration = 0;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public void Long_Category_FailsValidation()
    {
        var dto = ValidDto();
        dto.Category = new string('a', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Empty_ExecutorId_FailsValidation()
    {
        var dto = ValidDto();
        dto.ExecutorId = Guid.Empty;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.ExecutorId);
    }
}

public class UpdateServiceValidatorTests
{
    private readonly UpdateServiceValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateServiceDto
        {
            Id = Guid.NewGuid(),
            Name = "Service",
            Price = 100,
            Duration = 1,
            ExecutorId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateServiceDto
        {
            Id = Guid.Empty,
            Name = "Service",
            Price = 100,
            Duration = 1,
            ExecutorId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Short_Name_FailsValidation()
    {
        var dto = new UpdateServiceDto
        {
            Id = Guid.NewGuid(),
            Name = "AB",
            Price = 100,
            Duration = 1,
            ExecutorId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }
}
