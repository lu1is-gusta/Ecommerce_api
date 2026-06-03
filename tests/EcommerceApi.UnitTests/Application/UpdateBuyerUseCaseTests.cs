using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Application.UseCases.UpdateBuyer;
using EcommerceApi.Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace EcommerceApi.UnitTests.Application;

public class UpdateBuyerUseCaseTests
{
    private readonly Mock<IBuyerRepository> _repository = new();
    private readonly UpdateBuyerUseCase _useCase;

    public UpdateBuyerUseCaseTests()
    {
        _useCase = new UpdateBuyerUseCase(_repository.Object, new UpdateBuyerValidator());
    }

    [Fact]
    public async Task Updates_buyer_successfully()
    {
        var buyer = new Buyer("Old Name", "old@example.com");
        var request = new UpdateBuyerRequest("New Name", "new@example.com");

        // Same object returned for both fetches; after Update() it reflects the new values.
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(buyer);

        _repository
            .Setup(r => r.UpdateAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _useCase.ExecuteAsync(buyer.Id, request);

        Assert.Equal("New Name", response.Name);
        Assert.Equal("new@example.com", response.Email);
        _repository.Verify(r => r.UpdateAsync(It.IsAny<Buyer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_not_found_when_buyer_does_not_exist()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Buyer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), new UpdateBuyerRequest("Name", "mail@example.com")));
    }

    [Fact]
    public async Task Throws_validation_exception_when_email_is_invalid()
    {
        var request = new UpdateBuyerRequest("Valid Name", "not-an-email");

        await Assert.ThrowsAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), request));
    }
}
