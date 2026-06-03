using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetProducts;

public class GetProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProductResponse>> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repository.ListAsync(filter, cancellationToken);
        return new PagedResult<ProductResponse>(
            paged.Items.Select(p => p.ToResponse()).ToList(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount);
    }
}
