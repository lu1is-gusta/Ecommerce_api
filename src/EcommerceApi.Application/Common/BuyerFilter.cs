namespace EcommerceApi.Application.Common;

/// <summary>
/// Optional filters for listing buyers. Applied as conditional predicates in the repository.
/// </summary>
public class BuyerFilter
{
    public string? Name { get; init; }
    public string? Email { get; init; }
}
