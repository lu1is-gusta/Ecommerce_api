using EcommerceApi.Application.Common;

namespace EcommerceApi.Api.DTOs;

/// <summary>
/// Query-string filters for GET /products. Bound via [AsParameters].
/// </summary>
public class ProductQuery
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public ProductFilter ToFilter()
    {
        var page = Page ?? 1;
        var pageSize = PageSize ?? 20;
        return new()
        {
            Name = Name,
            MinPrice = MinPrice,
            MaxPrice = MaxPrice,
            Page = page < 1 ? 1 : page,
            PageSize = pageSize < 1 ? 1 : pageSize > 100 ? 100 : pageSize,
        };
    }
}
