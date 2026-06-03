using EcommerceApi.Application.Common;
using EcommerceApi.Domain.Entities;

namespace EcommerceApi.Application.Interfaces;

public interface IBuyerRepository
{
    Task<Buyer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Buyer>> ListAsync(BuyerFilter filter, CancellationToken cancellationToken = default);

    Task AddAsync(Buyer buyer, CancellationToken cancellationToken = default);

    Task UpdateAsync(Buyer buyer, CancellationToken cancellationToken = default);

    Task DeleteAsync(Buyer buyer, CancellationToken cancellationToken = default);

    Task<bool> HasOrdersAsync(Guid buyerId, CancellationToken cancellationToken = default);
}
