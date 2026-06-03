using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.CreateProduct;
using EcommerceApi.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class CreateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _repository = new();
    private readonly CreateProductUseCase _useCase;

    public CreateProductUseCaseTests()
    {
        _useCase = new CreateProductUseCase(_repository.Object, new CreateProductValidator());
    }

    [Fact]
    public async Task Creates_product_and_returns_response()
    {
        var request = new CreateProductRequest("Mechanical Keyboard", 150m);

        _repository
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product("Mechanical Keyboard", 150m));

        var response = await _useCase.ExecuteAsync(request);

        Assert.Equal("Mechanical Keyboard", response.Name);
        Assert.Equal(150m, response.Price);
        _repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_validation_exception_when_price_is_zero()
    {
        var request = new CreateProductRequest("Keyboard", 0m);

        await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecuteAsync(request));
        _repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_validation_exception_when_name_is_empty()
    {
        var request = new CreateProductRequest("", 50m);

        await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecuteAsync(request));
        _repository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
