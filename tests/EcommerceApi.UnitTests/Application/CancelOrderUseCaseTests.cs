using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.CancelOrder;
using EcommerceApi.Domain.Entities;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class CancelOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly CancelOrderUseCase _useCase;

    public CancelOrderUseCaseTests()
    {
        _useCase = new CancelOrderUseCase(_repository.Object);
    }

    [Fact]
    public async Task Cancels_an_existing_started_order()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var response = await _useCase.ExecuteAsync(order.Id);

        Assert.Equal("Cancelled", response.Status);
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
