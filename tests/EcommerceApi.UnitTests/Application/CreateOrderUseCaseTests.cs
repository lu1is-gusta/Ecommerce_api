using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.CreateOrder;
using EcommerceApi.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _repository = new();
    private readonly CreateOrderUseCase _useCase;

    public CreateOrderUseCaseTests()
    {
        _useCase = new CreateOrderUseCase(_repository.Object, new CreateOrderValidator());
    }

    private static CreateOrderRequest BuildRequest(Guid productId) =>
        new(new BuyerInput("Jane", "jane@example.com"),
            new[] { new OrderItemInput(productId, 2) });

    [Fact]
    public async Task Creates_order_and_uses_product_price_as_unit_price()
    {
        var product = new Product("Keyboard", 150m);
        var request = BuildRequest(product.Id);

        _repository
            .Setup(r => r.GetProductsByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { product });

        _repository
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order(new Buyer("Jane", "jane@example.com"),
                new[] { new OrderItem(product.Id, product.Price, 2) }));

        var response = await _useCase.ExecuteAsync(request);

        Assert.Equal("Started", response.Status);
        Assert.Single(response.Items);
        Assert.Equal(150m, response.Items[0].UnitPrice);
        Assert.Equal(300m, response.Total);
        _repository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_not_found_when_product_does_not_exist()
    {
        var request = BuildRequest(Guid.NewGuid());

        _repository
            .Setup(r => r.GetProductsByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Product>());

        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(request));
        _repository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_validation_exception_when_request_is_invalid()
    {
        var request = new CreateOrderRequest(
            new BuyerInput("", "not-an-email"),
            Array.Empty<OrderItemInput>());

        await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecuteAsync(request));
    }
}
