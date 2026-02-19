using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Validators.Auth;

namespace IT_outCRM.Tests.Validators;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Valid_Login_PassesValidation()
    {
        var dto = new LoginDto { Username = "admin", Password = "password123" };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Username_FailsValidation()
    {
        var dto = new LoginDto { Username = "", Password = "password123" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Short_Username_FailsValidation()
    {
        var dto = new LoginDto { Username = "ab", Password = "password123" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Empty_Password_FailsValidation()
    {
        var dto = new LoginDto { Username = "admin", Password = "" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Short_Password_FailsValidation()
    {
        var dto = new LoginDto { Username = "admin", Password = "12345" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Password);
    }
}
