using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.ShipOrder;

public class ShipOrderUseCase
{
    private readonly IOrderRepository _repository;

    public ShipOrderUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        // Domain enforces that only processed orders can be shipped.
        order.Ship();

        await _repository.UpdateAsync(order, cancellationToken);

        var shipped = await _repository.GetByIdAsync(order.Id, cancellationToken);
        return shipped!.ToResponse();
    }
}
