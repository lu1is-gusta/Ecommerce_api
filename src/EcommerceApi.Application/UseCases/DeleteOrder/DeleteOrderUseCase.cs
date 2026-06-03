using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.DeleteOrder;

public class DeleteOrderUseCase
{
    private readonly IOrderRepository _repository;

    public DeleteOrderUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        await _repository.DeleteAsync(order, cancellationToken);
    }
}
