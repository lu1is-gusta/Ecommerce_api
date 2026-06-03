using EcommerceApi.Domain.Enums;
using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.Entities;

/// <summary>
/// Aggregate root. All status transitions and invariants are encapsulated here,
/// so no outer layer can put an order into an invalid state.
/// </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public Guid BuyerId { get; private set; }
    public Buyer Buyer { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal Total => _items.Sum(i => i.Subtotal);

    private Order()
    {
    }

    public Order(Buyer buyer, IEnumerable<OrderItem> items)
    {
        if (buyer is null)
            throw new DomainException("An order must have a buyer.");

        Id = Guid.NewGuid();
        Buyer = buyer;
        BuyerId = buyer.Id;
        Status = OrderStatus.Started;
        CreatedAt = DateTime.UtcNow;

        AddItemsInternal(items);
        EnsureHasItems();
    }

    /// <summary>Replaces the items of the order. Only allowed while the order is editable.</summary>
    public void ReplaceItems(IEnumerable<OrderItem> items)
    {
        EnsureEditable();
        _items.Clear();
        AddItemsInternal(items);
        EnsureHasItems();
        Touch();
    }

    /// <summary>Started -> Processed.</summary>
    public void Process()
    {
        if (Status != OrderStatus.Started)
            throw new DomainException($"Only started orders can be processed. Current status: {Status}.");

        Status = OrderStatus.Processed;
        Touch();
    }

    /// <summary>Processed -> Shipped.</summary>
    public void Ship()
    {
        if (Status != OrderStatus.Processed)
            throw new DomainException($"Only processed orders can be shipped. Current status: {Status}.");

        Status = OrderStatus.Shipped;
        Touch();
    }

    /// <summary>Started or Processed -> Cancelled.</summary>
    public void Cancel()
    {
        if (Status is not (OrderStatus.Started or OrderStatus.Processed))
            throw new DomainException($"Only started or processed orders can be cancelled. Current status: {Status}.");

        Status = OrderStatus.Cancelled;
        Touch();
    }

    /// <summary>Only non-processed (i.e. still Started) orders may be edited.</summary>
    public bool IsEditable => Status == OrderStatus.Started;

    private void EnsureEditable()
    {
        if (!IsEditable)
            throw new DomainException($"Only orders that have not been processed can be modified. Current status: {Status}.");
    }

    private void AddItemsInternal(IEnumerable<OrderItem> items)
    {
        if (items is null)
            throw new DomainException("An order must have at least one product.");

        foreach (var item in items)
        {
            if (item is null)
                throw new DomainException("Order item cannot be null.");
            _items.Add(item);
        }
    }

    private void EnsureHasItems()
    {
        if (_items.Count == 0)
            throw new DomainException("An order must have at least one product.");
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
