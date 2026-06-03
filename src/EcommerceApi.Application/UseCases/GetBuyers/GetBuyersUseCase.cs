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

    public async Task<PagedResult<BuyerResponse>> ExecuteAsync(BuyerFilter filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repository.ListAsync(filter, cancellationToken);
        return new PagedResult<BuyerResponse>(
            paged.Items.Select(b => b.ToResponse()).ToList(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount);
    }
}
