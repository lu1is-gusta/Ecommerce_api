using EcommerceApi.Domain.Entities;
using EcommerceApi.Domain.Enums;
using EcommerceApi.Domain.Exceptions;
using Xunit;

namespace EcommerceApi.UnitTests.Domain;

public class OrderTests
{
    private static Buyer NewBuyer() => new("John Doe", "john@example.com");

    private static OrderItem NewItem() => new(Guid.NewGuid(), 10m, 1);

    private static Order NewStartedOrder() => new(NewBuyer(), new[] { NewItem() });

    [Fact]
    public void Creating_order_without_buyer_throws()
    {
        Assert.Throws<DomainException>(() => new Order(null!, new[] { NewItem() }));
    }

    [Fact]
    public void Creating_order_without_items_throws()
    {
        Assert.Throws<DomainException>(() => new Order(NewBuyer(), Array.Empty<OrderItem>()));
    }

    [Fact]
    public void New_order_starts_as_started_and_is_editable()
    {
        var order = NewStartedOrder();

        Assert.Equal(OrderStatus.Started, order.Status);
        Assert.True(order.IsEditable);
        Assert.Single(order.Items);
    }

    [Fact]
    public void Process_moves_started_order_to_processed()
    {
        var order = NewStartedOrder();

        order.Process();

        Assert.Equal(OrderStatus.Processed, order.Status);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void Process_twice_throws()
    {
        var order = NewStartedOrder();
        order.Process();

        Assert.Throws<DomainException>(() => order.Process());
    }

    [Fact]
    public void Ship_requires_processed_status()
    {
        var order = NewStartedOrder();

        Assert.Throws<DomainException>(() => order.Ship());

        order.Process();
        order.Ship();

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Cancel_allowed_from_started()
    {
        var order = NewStartedOrder();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_allowed_from_processed()
    {
        var order = NewStartedOrder();
        order.Process();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_not_allowed_from_shipped()
    {
        var order = NewStartedOrder();
        order.Process();
        order.Ship();

        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Fact]
    public void Cancel_not_allowed_when_already_cancelled()
    {
        var order = NewStartedOrder();
        order.Cancel();

        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Fact]
    public void ReplaceItems_allowed_only_while_started()
    {
        var order = NewStartedOrder();

        order.ReplaceItems(new[] { NewItem(), NewItem() });
        Assert.Equal(2, order.Items.Count);

        order.Process();
        Assert.Throws<DomainException>(() => order.ReplaceItems(new[] { NewItem() }));
    }

    [Fact]
    public void ReplaceItems_with_empty_collection_throws()
    {
        var order = NewStartedOrder();

        Assert.Throws<DomainException>(() => order.ReplaceItems(Array.Empty<OrderItem>()));
    }

    [Fact]
    public void Total_is_sum_of_subtotals()
    {
        var order = new Order(NewBuyer(), new[]
        {
            new OrderItem(Guid.NewGuid(), 10m, 2),
            new OrderItem(Guid.NewGuid(), 5m, 3)
        });

        Assert.Equal(35m, order.Total);
    }
}
