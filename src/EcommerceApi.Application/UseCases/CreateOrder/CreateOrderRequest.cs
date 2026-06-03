using EcommerceApi.Application.Common;

namespace EcommerceApi.Application.UseCases.CreateOrder;

public record CreateOrderRequest(BuyerInput Buyer, IReadOnlyList<OrderItemInput> Items);
