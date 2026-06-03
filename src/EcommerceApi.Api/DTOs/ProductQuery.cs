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

    public ProductFilter ToFilter() => new()
    {
        Name = Name,
        MinPrice = MinPrice,
        MaxPrice = MaxPrice,
    };
}
