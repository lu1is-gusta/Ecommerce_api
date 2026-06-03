using System.Net;
using System.Net.Http.Json;
using EcommerceApi.Application.Common;
using EcommerceApi.Infrastructure.Persistence.Configurations;
using Xunit;

namespace EcommerceApi.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<EcommerceApiFactory>
{
    private readonly HttpClient _client;

    public OrderEndpointsTests(EcommerceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object BuildCreatePayload(int quantity = 2) => new
    {
        buyer = new { name = "Jane Doe", email = "jane@example.com" },
        items = new[] { new { productId = SeedIds.Keyboard, quantity } }
    };

    [Fact]
    public async Task Create_order_returns_201_with_computed_total()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/orders", BuildCreatePayload());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(order);
        Assert.Equal("Started", order!.Status);
        Assert.Single(order.Items);
        Assert.Equal(150m, order.Items[0].UnitPrice);
        Assert.Equal(300m, order.Total);
        Assert.Equal("Jane Doe", order.Buyer.Name);
    }

    [Fact]
    public async Task Create_order_with_unknown_product_returns_404()
    {
        var payload = new
        {
            buyer = new { name = "Jane", email = "jane@example.com" },
            items = new[] { new { productId = Guid.NewGuid(), quantity = 1 } }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/orders", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_order_without_items_returns_400()
    {
        var payload = new
        {
            buyer = new { name = "Jane", email = "jane@example.com" },
            items = Array.Empty<object>()
        };

        var response = await _client.PostAsJsonAsync("/api/v1/orders", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_unknown_order_returns_404()
    {
        var response = await _client.GetAsync($"/api/v1/orders/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_returns_created_order_and_supports_status_filter()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/orders", BuildCreatePayload()))
            .Content.ReadFromJsonAsync<OrderResponse>();

        var all = await _client.GetFromJsonAsync<List<OrderResponse>>("/api/v1/orders");
        Assert.NotNull(all);
        Assert.Contains(all!, o => o.Id == created!.Id);

        var started = await _client.GetFromJsonAsync<List<OrderResponse>>("/api/v1/orders?status=Started");
        Assert.NotNull(started);
        Assert.All(started!, o => Assert.Equal("Started", o.Status));
    }

    [Fact]
    public async Task Cancel_order_changes_status_to_cancelled()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/orders", BuildCreatePayload()))
            .Content.ReadFromJsonAsync<OrderResponse>();

        var response = await _client.PutAsync($"/api/v1/orders/{created!.Id}/cancel", content: null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cancelled = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.Equal("Cancelled", cancelled!.Status);
    }

    [Fact]
    public async Task Delete_order_removes_it()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/orders", BuildCreatePayload()))
            .Content.ReadFromJsonAsync<OrderResponse>();

        var delete = await _client.DeleteAsync($"/api/v1/orders/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var get = await _client.GetAsync($"/api/v1/orders/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
