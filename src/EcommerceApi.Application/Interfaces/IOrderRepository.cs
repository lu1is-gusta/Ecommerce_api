using EcommerceApi.Application.Common;
using EcommerceApi.Domain.Entities;

namespace EcommerceApi.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Order>> ListAsync(OrderFilter filter, CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    Task DeleteAsync(Order order, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default);
}
