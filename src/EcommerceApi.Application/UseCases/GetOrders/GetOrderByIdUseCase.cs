using EcommerceApi.Application.Common;
using EcommerceApi.Application.Exceptions;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetOrders;

public class GetOrderByIdUseCase
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderResponse> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{orderId}' was not found.");

        return order.ToResponse();
    }
}
