using EcommerceApi.Application.Common;

namespace EcommerceApi.Api.DTOs;

/// <summary>
/// Query-string filters for GET /buyers. Bound via [AsParameters].
/// </summary>
public class BuyerQuery
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public BuyerFilter ToFilter()
    {
        var page = Page ?? 1;
        var pageSize = PageSize ?? 20;
        return new()
        {
            Name = Name,
            Email = Email,
            Page = page < 1 ? 1 : page,
            PageSize = pageSize < 1 ? 1 : pageSize > 100 ? 100 : pageSize,
        };
    }
}
