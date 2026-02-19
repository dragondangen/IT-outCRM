using FluentAssertions;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Services;
using Moq;

namespace IT_outCRM.Tests.Services;

public class EntityValidationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly EntityValidationService _sut;

    public EntityValidationServiceTests()
    {
        _sut = new EntityValidationService(_unitOfWork.Object);
    }

    // --- Account ---

    [Fact]
    public async Task EnsureAccountExistsAsync_WhenExists_DoesNotThrow()
    {
        var id = Guid.NewGuid();
        var accountRepo = new Mock<IAccountRepository>();
        accountRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _unitOfWork.Setup(u => u.Accounts).Returns(accountRepo.Object);

        var act = () => _sut.EnsureAccountExistsAsync(id);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureAccountExistsAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var accountRepo = new Mock<IAccountRepository>();
        accountRepo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        _unitOfWork.Setup(u => u.Accounts).Returns(accountRepo.Object);

        var act = () => _sut.EnsureAccountExistsAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    // --- Customer ---

    [Fact]
    public async Task EnsureCustomerExistsAsync_WhenExists_DoesNotThrow()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _unitOfWork.Setup(u => u.Customers).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureCustomerExistsAsync(id))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureCustomerExistsAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ICustomerRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        _unitOfWork.Setup(u => u.Customers).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureCustomerExistsAsync(id))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    // --- Company ---

    [Fact]
    public async Task EnsureCompanyExistsAsync_WhenExists_DoesNotThrow()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ICompanyRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _unitOfWork.Setup(u => u.Companies).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureCompanyExistsAsync(id))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureCompanyExistsAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<ICompanyRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        _unitOfWork.Setup(u => u.Companies).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureCompanyExistsAsync(id))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    // --- Executor ---

    [Fact]
    public async Task EnsureExecutorExistsAsync_WhenExists_DoesNotThrow()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IExecutorRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _unitOfWork.Setup(u => u.Executors).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureExecutorExistsAsync(id))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureExecutorExistsAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IExecutorRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        _unitOfWork.Setup(u => u.Executors).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureExecutorExistsAsync(id))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    // --- Order ---

    [Fact]
    public async Task EnsureOrderExistsAsync_WhenExists_DoesNotThrow()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IOrderRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _unitOfWork.Setup(u => u.Orders).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureOrderExistsAsync(id))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureOrderExistsAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var repo = new Mock<IOrderRepository>();
        repo.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        _unitOfWork.Setup(u => u.Orders).Returns(repo.Object);

        await _sut.Invoking(s => s.EnsureOrderExistsAsync(id))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}
