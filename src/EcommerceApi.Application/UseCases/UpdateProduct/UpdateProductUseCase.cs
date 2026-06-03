using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.UpdateProduct;

public class UpdateProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IValidator<UpdateProductRequest> _validator;

    public UpdateProductUseCase(IProductRepository repository, IValidator<UpdateProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ProductResponse> ExecuteAsync(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _repository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException($"Product '{productId}' was not found.");

        product.Update(request.Name, request.Price);
        await _repository.UpdateAsync(product, cancellationToken);

        var updated = await _repository.GetByIdAsync(product.Id, cancellationToken);
        return updated!.ToResponse();
    }
}
