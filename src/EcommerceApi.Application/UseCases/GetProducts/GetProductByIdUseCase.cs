using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetProducts;

public class GetProductByIdUseCase
{
    private readonly IProductRepository _repository;

    public GetProductByIdUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponse> ExecuteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product '{productId}' was not found.");

        return product.ToResponse();
    }
}
