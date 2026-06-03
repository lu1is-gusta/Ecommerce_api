using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.CreateBuyer;
using EcommerceApi.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class CreateBuyerUseCaseTests
{
    private readonly Mock<IBuyerRepository> _repository = new();
    private readonly CreateBuyerUseCase _useCase;

    public CreateBuyerUseCaseTests()
    {
        _useCase = new CreateBuyerUseCase(_repository.Object, new CreateBuyerValidator());
    }

    [Fact]
    public async Task Creates_buyer_and_returns_response()
    {
        var request = new CreateBuyerRequest("Jane Doe", "jane@example.com");

        _repository
            .Setup(r => r.AddAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Buyer("Jane Doe", "jane@example.com"));

        var response = await _useCase.ExecuteAsync(request);

        Assert.Equal("Jane Doe", response.Name);
        Assert.Equal("jane@example.com", response.Email);
        _repository.Verify(r => r.AddAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_validation_exception_when_email_is_invalid()
    {
        var request = new CreateBuyerRequest("Jane", "not-an-email");

        await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecuteAsync(request));
        _repository.Verify(r => r.AddAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Throws_validation_exception_when_name_is_empty()
    {
        var request = new CreateBuyerRequest("", "jane@example.com");

        await Assert.ThrowsAsync<ValidationException>(() => _useCase.ExecuteAsync(request));
        _repository.Verify(r => r.AddAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
