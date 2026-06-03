using EcommerceApi.Domain.Entities;

namespace EcommerceApi.Application.Common;

public record ProductResponse(Guid Id, string Name, decimal Price);

public static class ProductMapping
{
    public static ProductResponse ToResponse(this Product product) =>
        new(product.Id, product.Name, product.Price);
}
