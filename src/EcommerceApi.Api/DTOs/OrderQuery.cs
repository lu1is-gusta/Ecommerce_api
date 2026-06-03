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
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public OrderFilter ToFilter()
    {
        var page = Page ?? 1;
        var pageSize = PageSize ?? 20;
        return new()
        {
            Status = Status,
            BuyerId = BuyerId,
            From = From,
            To = To,
            Page = page < 1 ? 1 : page,
            PageSize = pageSize < 1 ? 1 : pageSize > 100 ? 100 : pageSize,
        };
    }
}
