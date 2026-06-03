using EcommerceApi.Domain.Exceptions;

namespace EcommerceApi.Domain.Entities;

public class Buyer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    private Buyer()
    {
    }

    public Buyer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Buyer name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Buyer email is required.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Buyer name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Buyer email is required.");

        Name = name.Trim();
        Email = email.Trim();
    }
}
