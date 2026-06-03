namespace EcommerceApi.Application.Common;

/// <summary>Buyer data supplied when creating an order.</summary>
public record BuyerInput(string Name, string Email);

/// <summary>A single product line supplied when creating or updating an order.</summary>
public record OrderItemInput(Guid ProductId, int Quantity);
