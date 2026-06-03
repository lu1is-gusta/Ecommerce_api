namespace EcommerceApi.Domain.Exceptions;

/// <summary>
/// Raised when a business invariant or a status transition rule is violated.
/// Lives in the Domain so the aggregate can protect its own invariants
/// without depending on any outer layer.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
