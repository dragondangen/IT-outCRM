using AutoMapper;
using FluentAssertions;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace IT_outCRM.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IContactPersonRepository> _contactPersonRepo = new();
    private readonly Mock<ICompanyRepository> _companyRepo = new();
    private readonly Mock<IAccountRepository> _accountRepo = new();
    private readonly Mock<IAccountStatusRepository> _accountStatusRepo = new();
    private readonly Mock<ICustomerRepository> _customerRepo = new();
    private readonly Mock<IExecutorRepository> _executorRepo = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWork.Setup(u => u.Users).Returns(_userRepo.Object);
        _unitOfWork.Setup(u => u.ContactPersons).Returns(_contactPersonRepo.Object);
        _unitOfWork.Setup(u => u.Companies).Returns(_companyRepo.Object);
        _unitOfWork.Setup(u => u.Accounts).Returns(_accountRepo.Object);
        _unitOfWork.Setup(u => u.AccountStatuses).Returns(_accountStatusRepo.Object);
        _unitOfWork.Setup(u => u.Customers).Returns(_customerRepo.Object);
        _unitOfWork.Setup(u => u.Executors).Returns(_executorRepo.Object);

        _configuration.Setup(c => c["Jwt:ExpirationHours"]).Returns("24");

        _sut = new AuthService(_unitOfWork.Object, _jwtService.Object, _mapper.Object, _configuration.Object);
    }

    // --- Register ---

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ThrowsInvalidOperationException()
    {
        _userRepo.Setup(r => r.UsernameExistsAsync("admin")).ReturnsAsync(true);

        var dto = new RegisterDto { Username = "admin", Email = "a@b.com", Password = "Pass123!" };
        var act = () => _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*именем*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepo.Setup(r => r.EmailExistsAsync("dup@test.com")).ReturnsAsync(true);

        var dto = new RegisterDto { Username = "user1", Email = "dup@test.com", Password = "Pass123!" };
        var act = () => _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*email*");
    }

    [Fact]
    public async Task RegisterAsync_SimpleUser_ReturnsToken()
    {
        _userRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _unitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _jwtService.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        var dto = new RegisterDto { Username = "newuser", Email = "new@test.com", Password = "Pass123!", Role = "User" };
        var result = await _sut.RegisterAsync(dto);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.Username.Should().Be("newuser");
        result.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task RegisterAsync_WithCompany_InvalidUserType_ThrowsArgumentException()
    {
        _userRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _contactPersonRepo.Setup(r => r.AddAsync(It.IsAny<ContactPerson>())).ReturnsAsync((ContactPerson c) => c);
        _companyRepo.Setup(r => r.AddAsync(It.IsAny<Company>())).ReturnsAsync((Company c) => c);
        _accountStatusRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AccountStatus>
            { new() { Id = Guid.NewGuid(), Name = "Active" } });
        _accountRepo.Setup(r => r.AddAsync(It.IsAny<Account>())).ReturnsAsync((Account a) => a);
        _unitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

        var dto = new RegisterDto
        {
            Username = "user1",
            Email = "u@t.com",
            Password = "Pass123!",
            CompanyName = "TestCo",
            UserType = "InvalidType"
        };

        var act = () => _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*UserType*");
    }

    // --- Login ---

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ThrowsUnauthorizedAccessException()
    {
        _userRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

        var dto = new LoginDto { Username = "unknown", Password = "pass" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "a@b.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPass"),
            IsActive = true
        };
        _userRepo.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);

        var dto = new LoginDto { Username = "admin", Password = "WrongPass" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveAccount_ThrowsUnauthorizedAccessException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "a@b.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            IsActive = false
        };
        _userRepo.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);

        var dto = new LoginDto { Username = "admin", Password = "Pass123!" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*деактивирована*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Role = "Admin",
            IsActive = true
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
        _userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _jwtService.Setup(j => j.GenerateToken(user)).Returns("jwt-token");

        var dto = new LoginDto { Username = "admin", Password = "Pass123!" };
        var result = await _sut.LoginAsync(dto);

        result.Token.Should().Be("jwt-token");
        result.Username.Should().Be("admin");
        result.Role.Should().Be("Admin");
        user.LastLoginAt.Should().NotBeNull();
    }

    // --- GetUserByIdAsync ---

    [Fact]
    public async Task GetUserByIdAsync_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, Username = "admin" };
        var dto = new UserDto { Id = id, Username = "admin" };

        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
        _mapper.Setup(m => m.Map<UserDto>(user)).Returns(dto);

        var result = await _sut.GetUserByIdAsync(id);

        result.Should().NotBeNull();
        result!.Username.Should().Be("admin");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenNotExists_ReturnsNull()
    {
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var result = await _sut.GetUserByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    // --- DeleteUserAsync ---

    [Fact]
    public async Task DeleteUserAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var act = () => _sut.DeleteUserAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteUserAsync_WhenExists_Deletes()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id };

        _userRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(user);
        _userRepo.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteUserAsync(id);

        _userRepo.Verify(r => r.DeleteAsync(user), Times.Once);
    }

    // --- GetAllUsersAsync ---

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        var users = new List<User> { new() { Id = Guid.NewGuid() }, new() { Id = Guid.NewGuid() } };
        var dtos = new List<UserDto> { new(), new() };

        _userRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
        _mapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(dtos);

        var result = await _sut.GetAllUsersAsync();

        result.Should().HaveCount(2);
    }
}
