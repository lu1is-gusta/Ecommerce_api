using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.ProcessOrder;
using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Exceptions;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class ProcessOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly ProcessOrderUseCase _useCase;

    public ProcessOrderUseCaseTests()
    {
        _useCase = new ProcessOrderUseCase(_repository.Object);
    }

    [Fact]
    public async Task Processes_an_existing_started_order()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var response = await _useCase.ExecuteAsync(order.Id);

        Assert.Equal("Processed", response.Status);
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_already_processed()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });
        order.Process();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id));
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_shipped()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });
        order.Process();
        order.Ship();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id));
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_domain_exception_when_order_is_cancelled()
    {
        var order = new Order(new Buyer("Jane", "jane@example.com"),
            new[] { new OrderItem(Guid.NewGuid(), 10m, 1) });
        order.Cancel();

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(order.Id));
        _repository.Verify(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Never);
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
