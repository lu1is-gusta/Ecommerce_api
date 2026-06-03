using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetBuyers;

public class GetBuyerByIdUseCase
{
    private readonly IBuyerRepository _repository;

    public GetBuyerByIdUseCase(IBuyerRepository repository)
    {
        _repository = repository;
    }

    public async Task<BuyerResponse> ExecuteAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        var buyer = await _repository.GetByIdAsync(buyerId, cancellationToken)
            ?? throw new NotFoundException($"Buyer '{buyerId}' was not found.");

        return buyer.ToResponse();
    }
}
