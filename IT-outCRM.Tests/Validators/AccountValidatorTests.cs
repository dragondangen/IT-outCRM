using FluentValidation.TestHelper;
using IT_outCRM.Application.DTOs.Account;
using IT_outCRM.Application.DTOs.AccountStatus;
using IT_outCRM.Application.Validators.Account;
using IT_outCRM.Application.Validators.AccountStatus;

namespace IT_outCRM.Tests.Validators;

public class CreateAccountValidatorTests
{
    private readonly CreateAccountValidator _validator = new();

    [Fact]
    public void Valid_Account_PassesValidation()
    {
        var dto = new CreateAccountDto
        {
            CompanyName = "Test",
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_CompanyName_FailsValidation()
    {
        var dto = new CreateAccountDto
        {
            CompanyName = "",
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Long_CompanyName_FailsValidation()
    {
        var dto = new CreateAccountDto
        {
            CompanyName = new string('a', 201),
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Fact]
    public void Future_FoundingDate_FailsValidation()
    {
        var dto = new CreateAccountDto
        {
            CompanyName = "Test",
            FoundingDate = DateTime.Now.AddYears(1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FoundingDate);
    }

    [Fact]
    public void Empty_AccountStatusId_FailsValidation()
    {
        var dto = new CreateAccountDto
        {
            CompanyName = "Test",
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.Empty
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.AccountStatusId);
    }
}

public class UpdateAccountValidatorTests
{
    private readonly UpdateAccountValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateAccountDto
        {
            Id = Guid.NewGuid(),
            CompanyName = "Test",
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateAccountDto
        {
            Id = Guid.Empty,
            CompanyName = "Test",
            FoundingDate = DateTime.Now.AddYears(-1),
            AccountStatusId = Guid.NewGuid()
        };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }
}

public class CreateAccountStatusValidatorTests
{
    private readonly CreateAccountStatusValidator _validator = new();

    [Fact]
    public void Valid_Status_PassesValidation()
    {
        var dto = new CreateAccountStatusDto { Name = "Active" };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Name_FailsValidation()
    {
        var dto = new CreateAccountStatusDto { Name = "" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Long_Name_FailsValidation()
    {
        var dto = new CreateAccountStatusDto { Name = new string('a', 101) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }
}

public class UpdateAccountStatusValidatorTests
{
    private readonly UpdateAccountStatusValidator _validator = new();

    [Fact]
    public void Valid_Update_PassesValidation()
    {
        var dto = new UpdateAccountStatusDto { Id = Guid.NewGuid(), Name = "Active" };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_Id_FailsValidation()
    {
        var dto = new UpdateAccountStatusDto { Id = Guid.Empty, Name = "Active" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Empty_Name_FailsValidation()
    {
        var dto = new UpdateAccountStatusDto { Id = Guid.NewGuid(), Name = "" };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }
}
