using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetBuyers;

public class GetBuyersUseCase
{
    private readonly IBuyerRepository _repository;

    public GetBuyersUseCase(IBuyerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BuyerResponse>> ExecuteAsync(BuyerFilter filter, CancellationToken cancellationToken = default)
    {
        var buyers = await _repository.ListAsync(filter, cancellationToken);
        return buyers.Select(b => b.ToResponse()).ToList();
    }
}
