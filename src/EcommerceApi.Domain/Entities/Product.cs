using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        if (price <= 0)
            throw new DomainException("Product price must be greater than zero.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Price = price;
    }
}
