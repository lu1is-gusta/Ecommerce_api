using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.CancelOrder;

public class CancelOrderUseCase
{
    private readonly IOrderRepository _repository;

    public CancelOrderUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        // Domain enforces that only started or processed orders can be cancelled.
        order.Cancel();

        await _repository.UpdateAsync(order, cancellationToken);

        var cancelled = await _repository.GetByIdAsync(order.Id, cancellationToken);
        return cancelled!.ToResponse();
    }
}
