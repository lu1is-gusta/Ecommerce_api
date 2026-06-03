using EcommerceApi.Domain.Entities;

namespace EcommerceApi.Application.Common;

public static class BuyerMapping
{
    public static BuyerResponse ToResponse(this Buyer buyer) =>
        new(buyer.Id, buyer.Name, buyer.Email);
}
