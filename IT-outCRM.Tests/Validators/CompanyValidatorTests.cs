using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Company;
using IT_outCRM.Application.Validators.Company;

namespace IT_outCRM.Tests.Validators;

public class CreateCompanyValidatorTests
{
    private readonly CreateCompanyValidator _validator = new();

    private CreateCompanyDto ValidDto() => new()
    {
        Name = "Test Company",
        Inn = "1234567890",
        LegalForm = "OOO",
        ContactPersonId = Guid.NewGuid()
    };

    [Fact]
    public void Valid_Company_PassesValidation()
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
    public void Long_Name_FailsValidation()
    {
        var dto = ValidDto();
        dto.Name = new string('a', 201);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    public void Valid_Inn_PassesValidation(string inn)
    {
        var dto = ValidDto();
        dto.Inn = inn;
        _validator.TestValidate(dto).ShouldNotHaveValidationErrorFor(x => x.Inn);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("abcdefghij")]
    [InlineData("1234567890123")]
    public void Invalid_Inn_FailsValidation(string inn)
    {
        var dto = ValidDto();
        dto.Inn = inn;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Inn);
    }

    [Fact]
    public void Empty_LegalForm_FailsValidation()
    {
        var dto = ValidDto();
        dto.LegalForm = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.LegalForm);
    }

    [Fact]
    public void Empty_ContactPersonId_FailsValidation()
    {
        var dto = ValidDto();
        dto.ContactPersonId = Guid.Empty;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.ContactPersonId);
    }
}

public class UpdateCompanyValidatorTests
{
    private readonly UpdateCompanyValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateCompanyDto
        {
            Id = Guid.NewGuid(),
            Name = "Company",
            Inn = "1234567890",
            LegalForm = "OOO",
            ContactPersonId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateCompanyDto
        {
            Id = Guid.Empty,
            Name = "Company",
            Inn = "1234567890",
            LegalForm = "OOO",
            ContactPersonId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }
}
