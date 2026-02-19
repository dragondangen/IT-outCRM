using FluentAssertions;
using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Validators.Auth;

namespace IT_outCRM.Tests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    private RegisterDto ValidDto() => new()
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "Password1!",
        Role = "User"
    };

    [Fact]
    public void Valid_Registration_PassesValidation()
    {
        var result = _validator.TestValidate(ValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- Username ---

    [Fact]
    public void Empty_Username_FailsValidation()
    {
        var dto = ValidDto();
        dto.Username = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Short_Username_FailsValidation()
    {
        var dto = ValidDto();
        dto.Username = "ab";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Long_Username_FailsValidation()
    {
        var dto = ValidDto();
        dto.Username = new string('a', 51);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Username_WithSpecialChars_FailsValidation()
    {
        var dto = ValidDto();
        dto.Username = "user@name";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Username_WithUnderscore_PassesValidation()
    {
        var dto = ValidDto();
        dto.Username = "test_user";
        _validator.TestValidate(dto).ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    // --- Email ---

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

    // --- Password ---

    [Fact]
    public void Empty_Password_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Short_Password_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "Ab1!";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_WithoutUppercase_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "password1!";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_WithoutLowercase_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "PASSWORD1!";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_WithoutDigit_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "Password!";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_WithoutSpecialChar_FailsValidation()
    {
        var dto = ValidDto();
        dto.Password = "Password1";
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    // --- Role ---

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("Manager")]
    public void Valid_Roles_PassValidation(string role)
    {
        var dto = ValidDto();
        dto.Role = role;
        _validator.TestValidate(dto).ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    [Theory]
    [InlineData("Executor")]
    [InlineData("SuperAdmin")]
    [InlineData("")]
    public void Invalid_Roles_FailValidation(string role)
    {
        var dto = ValidDto();
        dto.Role = role;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Role);
    }
}
