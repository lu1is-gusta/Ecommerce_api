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

    public async Task<PagedResult<OrderResponse>> ExecuteAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        var paged = await _repository.ListAsync(filter, cancellationToken);
        return new PagedResult<OrderResponse>(
            paged.Items.Select(o => o.ToResponse()).ToList(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount);
    }
}
