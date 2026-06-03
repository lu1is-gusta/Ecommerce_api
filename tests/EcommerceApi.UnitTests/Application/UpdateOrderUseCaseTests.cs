using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.UpdateOrder;
using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Exceptions;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class UpdateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly UpdateOrderUseCase _useCase;
    private readonly Product _product = new("Keyboard", 150m);

    public UpdateOrderUseCaseTests()
    {
        _useCase = new UpdateOrderUseCase(_repository.Object, new UpdateOrderValidator());
        _repository
            .Setup(r => r.GetProductsByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { _product });
    }

    private UpdateOrderRequest BuildRequest() =>
        new(new[] { new OrderItemInput(_product.Id, 2) });

    private Order NewStartedOrder() =>
        new(new Buyer("Jane", "jane@example.com"), new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });

    [Fact]
    public async Task Updates_items_of_a_started_order()
    {
        var order = NewStartedOrder();
        var updatedOrder = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(_product.Id, _product.Price, 2) });

        _repository
            .SetupSequence(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order)
            .ReturnsAsync(updatedOrder);

        var response = await _useCase.ExecuteAsync(order.Id, BuildRequest());

        Assert.Equal("Started", response.Status);
        Assert.Single(response.Items);
        Assert.Equal(150m, response.Items[0].UnitPrice);
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_not_found_when_order_is_missing()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(Guid.NewGuid(), BuildRequest()));
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_processed()
    {
        var order = NewStartedOrder();
        order.Process();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id, BuildRequest()));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_shipped()
    {
        var order = NewStartedOrder();
        order.Process();
        order.Ship();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id, BuildRequest()));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_cancelled()
    {
        var order = NewStartedOrder();
        order.Cancel();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id, BuildRequest()));
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
