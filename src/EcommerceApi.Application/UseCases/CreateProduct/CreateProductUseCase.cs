using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.CreateProduct;

public class CreateProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductUseCase(IProductRepository repository, IValidator<CreateProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ProductResponse> ExecuteAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = new Product(request.Name, request.Price);
        await _repository.AddAsync(product, cancellationToken);

        var created = await _repository.GetByIdAsync(product.Id, cancellationToken);
        return created!.ToResponse();
    }
}
