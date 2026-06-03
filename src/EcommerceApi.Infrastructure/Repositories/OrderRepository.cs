using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using EcommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Order>> ListAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Buyer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product);

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == filter.Status.Value);

        if (filter.BuyerId.HasValue)
            query = query.Where(o => o.BuyerId == filter.BuyerId.Value);

        if (filter.From.HasValue)
            query = query.Where(o => o.CreatedAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(o => o.CreatedAt <= filter.To.Value);

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Order>(items, filter.Page, filter.PageSize, totalCount);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        // The order is already tracked (loaded via GetByIdAsync); persist the change set.
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByIdsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();

        return await _context.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}
