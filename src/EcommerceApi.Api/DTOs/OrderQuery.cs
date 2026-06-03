using EcommerceApi.Application.Common;
using EcommerceApi.Domain.Enums;

namespace EcommerceApi.Api.DTOs;

/// <summary>
/// Query-string filters for GET /orders. Bound via [AsParameters].
/// </summary>
public class OrderQuery
{
    public OrderStatus? Status { get; set; }
    public Guid? BuyerId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    public OrderFilter ToFilter() => new()
    {
        Status = Status,
        BuyerId = BuyerId,
        From = From,
        To = To
    };
}
