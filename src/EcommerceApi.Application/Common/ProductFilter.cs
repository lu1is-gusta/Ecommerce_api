namespace EcommerceApi.Application.Common;

/// <summary>
/// Optional filters for listing products. Applied as conditional predicates in the repository.
/// </summary>
public class ProductFilter
{
    public string? Name { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}
