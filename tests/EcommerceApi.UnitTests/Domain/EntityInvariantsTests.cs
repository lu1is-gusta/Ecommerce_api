using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Exceptions;
using Xunit;

namespace EcommerceApi.UnitTests.Domain;

public class EntityInvariantsTests
{
    [Theory]
    [InlineData("", "john@example.com")]
    [InlineData("  ", "john@example.com")]
    [InlineData("John", "")]
    public void Buyer_requires_name_and_email(string name, string email)
    {
        Assert.Throws<DomainException>(() => new Buyer(name, email));
    }

    [Fact]
    public void Product_requires_positive_price()
    {
        Assert.Throws<DomainException>(() => new Product("Item", 0m));
        Assert.Throws<DomainException>(() => new Product("Item", -5m));
    }

    [Fact]
    public void Product_requires_name()
    {
        Assert.Throws<DomainException>(() => new Product(" ", 10m));
    }

    [Fact]
    public void OrderItem_requires_positive_quantity_and_price()
    {
        Assert.Throws<DomainException>(() => new OrderItem(Guid.NewGuid(), 10m, 0));
        Assert.Throws<DomainException>(() => new OrderItem(Guid.NewGuid(), 0m, 1));
        Assert.Throws<DomainException>(() => new OrderItem(Guid.Empty, 10m, 1));
    }

    [Fact]
    public void OrderItem_subtotal_is_unit_price_times_quantity()
    {
        var item = new OrderItem(Guid.NewGuid(), 12.5m, 4);

        Assert.Equal(50m, item.Subtotal);
    }
}
