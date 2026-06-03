using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.UpdateBuyer;

public class UpdateBuyerUseCase
{
    private readonly IBuyerRepository _repository;
    private readonly IValidator<UpdateBuyerRequest> _validator;

    public UpdateBuyerUseCase(IBuyerRepository repository, IValidator<UpdateBuyerRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<BuyerResponse> ExecuteAsync(Guid buyerId, UpdateBuyerRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var buyer = await _repository.GetByIdAsync(buyerId, cancellationToken)
            ?? throw new NotFoundException($"Buyer '{buyerId}' was not found.");

        buyer.Update(request.Name, request.Email);
        await _repository.UpdateAsync(buyer, cancellationToken);

        var updated = await _repository.GetByIdAsync(buyer.Id, cancellationToken);
        return updated!.ToResponse();
    }
}
