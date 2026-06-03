using EcommerceApi.Application.Common;

namespace EcommerceApi.Api.DTOs;

/// <summary>
/// Query-string filters for GET /buyers. Bound via [AsParameters].
/// </summary>
public class BuyerQuery
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public BuyerFilter ToFilter() => new()
    {
        Name = Name,
        Email = Email,
    };
}
