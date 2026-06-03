using EcommerceApi.Domain.Entities;

namespace EcommerceApi.Application.Common;

public record OrderResponse(
    Guid Id,
    BuyerResponse Buyer,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    decimal Total,
    IReadOnlyList<OrderItemResponse> Items);

public record BuyerResponse(Guid Id, string Name, string Email);

public record OrderItemResponse(
    Guid ProductId,
    string? ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

/// <summary>
/// Centralizes the Order -> OrderResponse projection (DRY) so every use case
/// returns the same shape.
/// </summary>
public static class OrderMapping
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            new BuyerResponse(order.Buyer.Id, order.Buyer.Name, order.Buyer.Email),
            order.Status.ToString(),
            order.CreatedAt,
            order.UpdatedAt,
            order.Total,
            order.Items
                .Select(i => new OrderItemResponse(
                    i.ProductId,
                    i.Product?.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.Subtotal))
                .ToList());
    }
}
