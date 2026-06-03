using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.UpdateOrder;

public class UpdateOrderUseCase
{
    private readonly IOrderRepository _repository;
    private readonly IValidator<UpdateOrderRequest> _validator;

    public UpdateOrderUseCase(IOrderRepository repository, IValidator<UpdateOrderRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
        var productsById = products.ToDictionary(p => p.Id);

        var items = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            if (!productsById.TryGetValue(item.ProductId, out var product))
                throw new NotFoundException($"Product '{item.ProductId}' was not found.");

            items.Add(new OrderItem(product.Id, product.Price, item.Quantity));
        }

        // Domain enforces that only editable (non-processed) orders can be changed.
        order.ReplaceItems(items);

        await _repository.UpdateAsync(order, cancellationToken);

        var updated = await _repository.GetByIdAsync(order.Id, cancellationToken);
        return updated!.ToResponse();
    }
}
