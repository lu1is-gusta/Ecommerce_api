using System.Net;
using System.Net.Http.Json;
using EcommerceApi.Application.Common;
using EcommerceApi.Infrastructure.Persistence.Configurations;
using Xunit;

namespace EcommerceApi.IntegrationTests;

public class BuyerEndpointsTests : IClassFixture<EcommerceApiFactory>
{
    private readonly HttpClient _client;

    public BuyerEndpointsTests(EcommerceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object BuildCreatePayload(string name = "Jane Doe", string email = "jane@example.com") => new
    {
        name,
        email
    };

    [Fact]
    public async Task Create_buyer_returns_201_with_correct_data()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/buyers", BuildCreatePayload("John Smith", "john@example.com"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var buyer = await response.Content.ReadFromJsonAsync<BuyerResponse>();
        Assert.NotNull(buyer);
        Assert.Equal("John Smith", buyer!.Name);
        Assert.Equal("john@example.com", buyer.Email);
        Assert.NotEqual(Guid.Empty, buyer.Id);
    }

    [Fact]
    public async Task Create_buyer_with_invalid_email_returns_400()
    {
        var payload = new { name = "Jane", email = "not-an-email" };

        var response = await _client.PostAsJsonAsync("/api/v1/buyers", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_buyer_with_empty_name_returns_400()
    {
        var payload = new { name = "", email = "valid@example.com" };

        var response = await _client.PostAsJsonAsync("/api/v1/buyers", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_buyer_by_id_returns_200()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/buyers", BuildCreatePayload()))
            .Content.ReadFromJsonAsync<BuyerResponse>();

        var response = await _client.GetAsync($"/api/v1/buyers/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var buyer = await response.Content.ReadFromJsonAsync<BuyerResponse>();
        Assert.Equal(created.Id, buyer!.Id);
    }

    [Fact]
    public async Task Get_unknown_buyer_returns_404()
    {
        var response = await _client.GetAsync($"/api/v1/buyers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_returns_created_buyer()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/buyers", BuildCreatePayload("List Test", "listtest@example.com")))
            .Content.ReadFromJsonAsync<BuyerResponse>();

        var paged = await _client.GetFromJsonAsync<PagedResult<BuyerResponse>>("/api/v1/buyers");

        Assert.NotNull(paged);
        Assert.Contains(paged!.Items, b => b.Id == created!.Id);
        Assert.True(paged.TotalCount > 0);
        Assert.Equal(1, paged.Page);
    }

    [Fact]
    public async Task Update_buyer_returns_200_with_new_values()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/buyers", BuildCreatePayload("Original", "original@example.com")))
            .Content.ReadFromJsonAsync<BuyerResponse>();

        var updatePayload = new { name = "Updated Name", email = "updated@example.com" };
        var response = await _client.PutAsJsonAsync($"/api/v1/buyers/{created!.Id}", updatePayload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<BuyerResponse>();
        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal("updated@example.com", updated.Email);
    }

    [Fact]
    public async Task Update_unknown_buyer_returns_404()
    {
        var payload = new { name = "Name", email = "mail@example.com" };

        var response = await _client.PutAsJsonAsync($"/api/v1/buyers/{Guid.NewGuid()}", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_buyer_without_orders_returns_204()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/buyers", BuildCreatePayload("To Delete", "todelete@example.com")))
            .Content.ReadFromJsonAsync<BuyerResponse>();

        var delete = await _client.DeleteAsync($"/api/v1/buyers/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var get = await _client.GetAsync($"/api/v1/buyers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_buyer_with_orders_returns_422()
    {
        var orderPayload = new
        {
            buyer = new { name = "Buyer With Order", email = "buyerwithorder@example.com" },
            items = new[] { new { productId = SeedIds.Keyboard, quantity = 1 } }
        };

        var order = await (await _client.PostAsJsonAsync("/api/v1/orders", orderPayload))
            .Content.ReadFromJsonAsync<OrderResponse>();

        var response = await _client.DeleteAsync($"/api/v1/buyers/{order!.Buyer.Id}");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Delete_unknown_buyer_returns_404()
    {
        var response = await _client.DeleteAsync($"/api/v1/buyers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
