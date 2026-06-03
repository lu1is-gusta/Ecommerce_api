using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Navigation populated only when an order is loaded with its products.
    /// On creation only the foreign key (<see cref="ProductId"/>) is set, so EF
    /// never tries to insert an already-existing product.
    /// </summary>
    public Product? Product { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Subtotal => UnitPrice * Quantity;

    private OrderItem()
    {
    }

    public OrderItem(Guid productId, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Order item must reference a product.");
        if (unitPrice <= 0)
            throw new DomainException("Order item unit price must be greater than zero.");
        if (quantity <= 0)
            throw new DomainException("Order item quantity must be greater than zero.");

        Id = Guid.NewGuid();
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
