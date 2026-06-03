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

    public async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var products = await _repository.ListAsync(filter, cancellationToken);
        return products.Select(p => p.ToResponse()).ToList();
    }
}
