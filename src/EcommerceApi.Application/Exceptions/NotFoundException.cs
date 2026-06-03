namespace EcommerceApi.Application.Exceptions;

/// <summary>
/// Raised by use cases when a requested resource does not exist.
/// Mapped to HTTP 404 by the global exception handler.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
