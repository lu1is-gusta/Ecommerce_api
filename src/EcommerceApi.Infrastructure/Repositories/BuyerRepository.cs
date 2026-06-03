using EcommerceApi.Application.Common;
using EcommerceApi.Application.Interfaces;
using EcommerceApi.Domain.Entities;
using EcommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Infrastructure.Repositories;

public class BuyerRepository : IBuyerRepository
{
    private readonly AppDbContext _context;

    public BuyerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Buyer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Buyers
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Buyer>> ListAsync(BuyerFilter filter, CancellationToken cancellationToken = default)
    {
        IQueryable<Buyer> query = _context.Buyers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(b => b.Name.Contains(filter.Name));

        if (!string.IsNullOrWhiteSpace(filter.Email))
            query = query.Where(b => b.Email.Contains(filter.Email));

        query = query.OrderBy(b => b.Name);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        await _context.Buyers.AddAsync(buyer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Buyer buyer, CancellationToken cancellationToken = default)
    {
        _context.Buyers.Remove(buyer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOrdersAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AnyAsync(o => o.BuyerId == buyerId, cancellationToken);
    }
}
