using AutoMapper;
using FluentAssertions;
using IT_outCRM.Application.DTOs.Order;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;
using Moq;

namespace IT_outCRM.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IOrderStatusRepository> _statusRepo = new();
    private readonly Mock<IEntityValidationService> _validationService = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _unitOfWork.Setup(u => u.Orders).Returns(_orderRepo.Object);
        _unitOfWork.Setup(u => u.OrderStatuses).Returns(_statusRepo.Object);
        _sut = new OrderService(_unitOfWork.Object, _mapper.Object, _validationService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var order = new Order { Id = id, Name = "Test Order" };
        var dto = new OrderDto { Id = id, Name = "Test Order" };

        _orderRepo.Setup(r => r.GetOrderWithDetailsAsync(id)).ReturnsAsync(order);
        _mapper.Setup(m => m.Map<OrderDto>(order)).Returns(dto);

        var result = await _sut.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Order");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        _orderRepo.Setup(r => r.GetOrderWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithoutCustomerId_ThrowsArgumentException()
    {
        var createDto = new CreateOrderDto
        {
            Name = "Order",
            Price = 100,
            CustomerId = null,
            OrderStatusId = Guid.NewGuid()
        };

        var act = () => _sut.CreateAsync(createDto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*CustomerId*");
    }

    [Fact]
    public async Task CreateAsync_WithEmptyCustomerId_ThrowsArgumentException()
    {
        var createDto = new CreateOrderDto
        {
            Name = "Order",
            Price = 100,
            CustomerId = Guid.Empty,
            OrderStatusId = Guid.NewGuid()
        };

        var act = () => _sut.CreateAsync(createDto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*CustomerId*");
    }

    [Fact]
    public async Task CreateAsync_WithoutOrderStatusId_ThrowsArgumentException()
    {
        var customerId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            Name = "Order",
            Price = 100,
            CustomerId = customerId,
            OrderStatusId = null
        };

        _validationService.Setup(v => v.EnsureCustomerExistsAsync(customerId)).Returns(Task.CompletedTask);

        var act = () => _sut.CreateAsync(createDto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*OrderStatusId*");
    }

    [Fact]
    public async Task CreateAsync_WithExecutorId_ValidatesExecutorExists()
    {
        var customerId = Guid.NewGuid();
        var executorId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            Name = "Order",
            Price = 100,
            CustomerId = customerId,
            ExecutorId = executorId,
            OrderStatusId = statusId
        };

        _validationService.Setup(v => v.EnsureCustomerExistsAsync(customerId)).Returns(Task.CompletedTask);
        _validationService.Setup(v => v.EnsureExecutorExistsAsync(executorId))
            .ThrowsAsync(new KeyNotFoundException("Executor not found"));

        var act = () => _sut.CreateAsync(createDto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _validationService.Verify(v => v.EnsureExecutorExistsAsync(executorId), Times.Once);
    }

    // --- TakeOrderAsync ---

    [Fact]
    public async Task TakeOrderAsync_WhenOrderNotFound_ThrowsKeyNotFoundException()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var act = () => _sut.TakeOrderAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task TakeOrderAsync_WhenAlreadyTaken_ThrowsInvalidOperationException()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, ExecutorId = Guid.NewGuid() };
        _orderRepo.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        var act = () => _sut.TakeOrderAsync(orderId, Guid.NewGuid());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already taken*");
    }

    [Fact]
    public async Task TakeOrderAsync_WhenAvailable_AssignsExecutor()
    {
        var orderId = Guid.NewGuid();
        var executorId = Guid.NewGuid();
        var order = new Order { Id = orderId, ExecutorId = null };
        var status = new OrderStatus { Id = Guid.NewGuid(), Name = "На рассмотрении" };

        _orderRepo.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _statusRepo.Setup(r => r.GetByNameAsync("На рассмотрении")).ReturnsAsync(status);
        _orderRepo.Setup(r => r.UpdateAsync(order)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.TakeOrderAsync(orderId, executorId);

        order.ExecutorId.Should().Be(executorId);
        order.OrderStatusId.Should().Be(status.Id);
        _orderRepo.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    [Fact]
    public async Task TakeOrderAsync_WhenStatusNotFound_AssignsExecutorWithoutStatusChange()
    {
        var orderId = Guid.NewGuid();
        var executorId = Guid.NewGuid();
        var originalStatusId = Guid.NewGuid();
        var order = new Order { Id = orderId, ExecutorId = null, OrderStatusId = originalStatusId };

        _orderRepo.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _statusRepo.Setup(r => r.GetByNameAsync("На рассмотрении")).ReturnsAsync((OrderStatus?)null);
        _orderRepo.Setup(r => r.UpdateAsync(order)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.TakeOrderAsync(orderId, executorId);

        order.ExecutorId.Should().Be(executorId);
        order.OrderStatusId.Should().Be(originalStatusId);
    }

    // --- GetStatusIdByNameAsync ---

    [Fact]
    public async Task GetStatusIdByNameAsync_WhenFound_ReturnsId()
    {
        var status = new OrderStatus { Id = Guid.NewGuid(), Name = "Active" };
        _statusRepo.Setup(r => r.GetByNameAsync("Active")).ReturnsAsync(status);

        var result = await _sut.GetStatusIdByNameAsync("Active");

        result.Should().Be(status.Id);
    }

    [Fact]
    public async Task GetStatusIdByNameAsync_WhenNotFound_ReturnsEmptyGuid()
    {
        _statusRepo.Setup(r => r.GetByNameAsync("Missing")).ReturnsAsync((OrderStatus?)null);

        var result = await _sut.GetStatusIdByNameAsync("Missing");

        result.Should().Be(Guid.Empty);
    }

    // --- GetOrdersByCustomerAsync ---

    [Fact]
    public async Task GetOrdersByCustomerAsync_ReturnsMappedOrders()
    {
        var customerId = Guid.NewGuid();
        var orders = new List<Order> { new() { Id = Guid.NewGuid(), Name = "O1" } };
        var dtos = new List<OrderDto> { new() { Name = "O1" } };

        _orderRepo.Setup(r => r.GetOrdersByCustomerAsync(customerId)).ReturnsAsync(orders);
        _mapper.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(dtos);

        var result = await _sut.GetOrdersByCustomerAsync(customerId);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOrdersByExecutorAsync_ReturnsMappedOrders()
    {
        var executorId = Guid.NewGuid();
        var orders = new List<Order> { new() { Id = Guid.NewGuid() } };
        var dtos = new List<OrderDto> { new() };

        _orderRepo.Setup(r => r.GetOrdersByExecutorAsync(executorId)).ReturnsAsync(orders);
        _mapper.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(dtos);

        var result = await _sut.GetOrdersByExecutorAsync(executorId);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDefaultSupportTeamIdAsync_WhenOrdersWithTeam_ReturnsId()
    {
        var teamId = Guid.NewGuid();
        var orders = new List<Order> { new() { SupportTeamId = teamId } };
        _orderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        var result = await _sut.GetDefaultSupportTeamIdAsync();

        result.Should().Be(teamId);
    }

    [Fact]
    public async Task GetDefaultSupportTeamIdAsync_WhenNoOrders_ReturnsEmptyGuid()
    {
        _orderRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Order>());

        var result = await _sut.GetDefaultSupportTeamIdAsync();

        result.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
