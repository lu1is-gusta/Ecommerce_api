namespace EcommerceApi.Domain.Enums;

/// <summary>
/// Lifecycle states of an order. Stored as int in the database.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order created and received (Iniciado).</summary>
    Started = 1,

    /// <summary>Order processed by the system (Processado).</summary>
    Processed = 2,

    /// <summary>Order shipped to the buyer (Enviado).</summary>
    Shipped = 3,

    /// <summary>Order cancelled by the buyer (Cancelado).</summary>
    Cancelled = 4
}
