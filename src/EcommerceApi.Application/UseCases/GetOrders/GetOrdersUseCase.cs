using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;

namespace EcommerceApi.Application.UseCases.GetOrders;

public class GetOrdersUseCase
{
    private readonly IOrderRepository _repository;

    public GetOrdersUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        var orders = await _repository.ListAsync(filter, cancellationToken);
        return orders.Select(o => o.ToResponse()).ToList();
    }
}
