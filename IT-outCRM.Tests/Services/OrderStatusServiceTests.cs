using AutoMapper;
using FluentAssertions;
using IT_outCRM.Application.DTOs.OrderStatus;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Repositories;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;
using Moq;

namespace IT_outCRM.Tests.Services;

public class OrderStatusServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IOrderStatusRepository> _orderStatusRepo = new();
    private readonly OrderStatusService _sut;

    public OrderStatusServiceTests()
    {
        _unitOfWork.Setup(u => u.OrderStatuses).Returns(_orderStatusRepo.Object);
        _sut = new OrderStatusService(_unitOfWork.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var entity = new OrderStatus { Id = id, Name = "Active" };
        var dto = new OrderStatusDto { Id = id, Name = "Active" };

        _orderStatusRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _mapper.Setup(m => m.Map<OrderStatusDto>(entity)).Returns(dto);

        var result = await _sut.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Active");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        _orderStatusRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrderStatus?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStatuses()
    {
        var entities = new List<OrderStatus>
        {
            new() { Id = Guid.NewGuid(), Name = "New" },
            new() { Id = Guid.NewGuid(), Name = "Active" }
        };
        var dtos = new List<OrderStatusDto>
        {
            new() { Id = entities[0].Id, Name = "New" },
            new() { Id = entities[1].Id, Name = "Active" }
        };

        _orderStatusRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapper.Setup(m => m.Map<IEnumerable<OrderStatusDto>>(entities)).Returns(dtos);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        var createDto = new CreateOrderStatusDto { Name = "Active" };
        _orderStatusRepo.Setup(r => r.NameExistsAsync("Active")).ReturnsAsync(true);

        var act = () => _sut.CreateAsync(createDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Active*");
    }

    [Fact]
    public async Task CreateAsync_WithUniqueName_CreatesSuccessfully()
    {
        var createDto = new CreateOrderStatusDto { Name = "New" };
        var entity = new OrderStatus { Id = Guid.NewGuid(), Name = "New" };
        var dto = new OrderStatusDto { Id = entity.Id, Name = "New" };

        _orderStatusRepo.Setup(r => r.NameExistsAsync("New")).ReturnsAsync(false);
        _mapper.Setup(m => m.Map<OrderStatus>(createDto)).Returns(entity);
        _orderStatusRepo.Setup(r => r.AddAsync(It.IsAny<OrderStatus>())).ReturnsAsync(entity);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _orderStatusRepo.Setup(r => r.GetByIdAsync(entity.Id)).ReturnsAsync(entity);
        _mapper.Setup(m => m.Map<OrderStatusDto>(entity)).Returns(dto);

        var result = await _sut.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.Name.Should().Be("New");
        _orderStatusRepo.Verify(r => r.AddAsync(It.IsAny<OrderStatus>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var updateDto = new UpdateOrderStatusDto { Id = id, Name = "Active" };
        var entity = new OrderStatus { Id = id, Name = "Old" };

        _orderStatusRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _orderStatusRepo.Setup(r => r.NameExistsAsync("Active", id)).ReturnsAsync(true);

        var act = () => _sut.UpdateAsync(updateDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Active*");
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _orderStatusRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrderStatus?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_DeletesSuccessfully()
    {
        var id = Guid.NewGuid();
        var entity = new OrderStatus { Id = id, Name = "Test" };

        _orderStatusRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _orderStatusRepo.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(id);

        _orderStatusRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        var entities = new List<OrderStatus> { new() { Id = Guid.NewGuid(), Name = "Test" } };
        var dtos = new List<OrderStatusDto> { new() { Name = "Test" } };

        _orderStatusRepo.Setup(r => r.GetPagedAsync(1, 10))
            .ReturnsAsync((entities.AsEnumerable(), 1));
        _mapper.Setup(m => m.Map<List<OrderStatusDto>>(entities)).Returns(dtos);

        var result = await _sut.GetPagedAsync(1, 10);

        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.PageNumber.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}
