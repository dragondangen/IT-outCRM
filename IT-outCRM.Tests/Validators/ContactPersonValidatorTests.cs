using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.ContactPerson;
using IT_outCRM.Application.Validators.ContactPerson;

namespace IT_outCRM.Tests.Validators;

public class CreateContactPersonValidatorTests
{
    private readonly CreateContactPersonValidator _validator = new();

    private CreateContactPersonDto ValidDto() => new()
    {
        FirstName = "Иван",
        LastName = "Иванов",
        MiddleName = "Иванович",
        Email = "ivan@test.com",
        PhoneNumber = "+79001234567",
        Role = "Director"
    };

    [Fact]
    public void Valid_ContactPerson_PassesValidation()
    {
        _validator.TestValidate(ValidDto()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_FirstName_FailsValidation()
    {
        var dto = ValidDto();
        dto.FirstName = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Long_FirstName_FailsValidation()
    {
        var dto = ValidDto();
        dto.FirstName = new string('a', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Empty_LastName_FailsValidation()
    {
        var dto = ValidDto();
        dto.LastName = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Long_MiddleName_FailsValidation()
    {
        var dto = ValidDto();
        dto.MiddleName = new string('a', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.MiddleName);
    }

    [Fact]
    public void Empty_Email_FailsValidation()
    {
        var dto = ValidDto();
        dto.Email = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Invalid_Email_FailsValidation()
    {
        var dto = ValidDto();
        dto.Email = "not-an-email";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Empty_PhoneNumber_FailsValidation()
    {
        var dto = ValidDto();
        dto.PhoneNumber = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("+79001234567")]
    [InlineData("89001234567")]
    [InlineData("+7 (900) 123-45-67")]
    [InlineData("8(900)123-45-67")]
    public void Valid_PhoneNumbers_PassValidation(string phone)
    {
        var dto = ValidDto();
        dto.PhoneNumber = phone;
        _validator.TestValidate(dto).ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcdefghij")]
    [InlineData("+1234567890")]
    public void Invalid_PhoneNumbers_FailValidation(string phone)
    {
        var dto = ValidDto();
        dto.PhoneNumber = phone;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}

public class UpdateContactPersonValidatorTests
{
    private readonly UpdateContactPersonValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateContactPersonDto
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            PhoneNumber = "+79001234567"
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateContactPersonDto
        {
            Id = Guid.Empty,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            PhoneNumber = "+79001234567"
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }
}
