using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.DeleteProduct;

public class DeleteProductUseCase
{
    private readonly IProductRepository _repository;

    public DeleteProductUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product '{productId}' was not found.");

        await _repository.DeleteAsync(product, cancellationToken);
    }
}
