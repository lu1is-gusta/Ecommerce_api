using EcommerceApi.Application.Common;

namespace EcommerceApi.Application.UseCases.UpdateOrder;

public record UpdateOrderRequest(IReadOnlyList<OrderItemInput> Items);
