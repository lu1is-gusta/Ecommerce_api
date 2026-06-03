using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.ShipOrder;
using EcommerceApi.Domain.Entities;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class ShipOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly ShipOrderUseCase _useCase;

    public ShipOrderUseCaseTests()
    {
        _useCase = new ShipOrderUseCase(_repository.Object);
    }

    [Fact]
    public async Task Ships_an_existing_processed_order()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });
        order.Process();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var response = await _useCase.ExecuteAsync(order.Id);

        Assert.Equal("Shipped", response.Status);
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_not_found_when_order_is_missing()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(Guid.NewGuid()));
    }
}
