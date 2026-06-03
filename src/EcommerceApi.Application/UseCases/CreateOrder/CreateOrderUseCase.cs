using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using FluentValidation;

namespace EcommerceApi.Application.UseCases.CreateOrder;

public class CreateOrderUseCase
{
    private readonly IOrderRepository _repository;
    private readonly IValidator<CreateOrderRequest> _validator;

    public CreateOrderUseCase(IOrderRepository repository, IValidator<CreateOrderRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

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

        var buyer = new Buyer(request.Buyer.Name, request.Buyer.Email);
        var order = new Order(buyer, items);

        await _repository.AddAsync(order, cancellationToken);

        var created = await _repository.GetByIdAsync(order.Id, cancellationToken);
        return created!.ToResponse();
    }
}
