using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.ProcessOrder;

public class ProcessOrderUseCase
{
    private readonly IOrderRepository _repository;

    public ProcessOrderUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        // Domain enforces that only started orders can be processed.
        order.Process();

        await _repository.UpdateAsync(order, cancellationToken);

        var processed = await _repository.GetByIdAsync(order.Id, cancellationToken);
        return processed!.ToResponse();
    }
}
