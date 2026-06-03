using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.UpdateProduct;
using EcommerceApi.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class UpdateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _repository = new();
    private readonly UpdateProductUseCase _useCase;

    public UpdateProductUseCaseTests()
    {
        _useCase = new UpdateProductUseCase(_repository.Object, new UpdateProductValidator());
    }

    [Fact]
    public async Task Updates_product_successfully()
    {
        var product = new Product("Old Name", 100m);
        var request = new UpdateProductRequest("New Name", 200m);

        // Same object returned for both fetches; after Update() it reflects the new values.
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repository
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _useCase.ExecuteAsync(product.Id, request);

        Assert.Equal("New Name", response.Name);
        Assert.Equal(200m, response.Price);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_not_found_when_product_does_not_exist()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), new UpdateProductRequest("Name", 10m)));
    }

    [Fact]
    public async Task Throws_validation_exception_when_price_is_negative()
    {
        var request = new UpdateProductRequest("Valid Name", -5m);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), request));
    }
}
