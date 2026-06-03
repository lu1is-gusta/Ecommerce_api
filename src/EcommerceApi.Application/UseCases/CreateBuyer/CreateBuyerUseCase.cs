using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.CreateBuyer;

public class CreateBuyerUseCase
{
    private readonly IBuyerRepository _repository;
    private readonly IValidator<CreateBuyerRequest> _validator;

    public CreateBuyerUseCase(IBuyerRepository repository, IValidator<CreateBuyerRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<BuyerResponse> ExecuteAsync(CreateBuyerRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var buyer = new Buyer(request.Name, request.Email);
        await _repository.AddAsync(buyer, cancellationToken);

        var created = await _repository.GetByIdAsync(buyer.Id, cancellationToken);
        return created!.ToResponse();
    }
}
