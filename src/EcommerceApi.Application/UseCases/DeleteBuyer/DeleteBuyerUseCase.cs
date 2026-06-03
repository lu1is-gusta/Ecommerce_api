using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Application.UseCases.DeleteBuyer;

public class DeleteBuyerUseCase
{
    private readonly IBuyerRepository _repository;

    public DeleteBuyerUseCase(IBuyerRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        var buyer = await _repository.GetByIdAsync(buyerId, cancellationToken)
            ?? throw new NotFoundException($"Buyer '{buyerId}' was not found.");

        if (await _repository.HasOrdersAsync(buyerId, cancellationToken))
            throw new DomainException("Cannot delete a buyer that has associated orders.");

        await _repository.DeleteAsync(buyer, cancellationToken);
    }
}
