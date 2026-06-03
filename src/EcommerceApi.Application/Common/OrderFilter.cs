using EcommerceApi.Domain.Enums;

namespace EcommerceApi.Application.Common;

/// <summary>
/// Optional filters for listing orders. Applied as conditional predicates in the repository.
/// </summary>
public class OrderFilter
{
    public OrderStatus? Status { get; init; }
    public Guid? BuyerId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}
