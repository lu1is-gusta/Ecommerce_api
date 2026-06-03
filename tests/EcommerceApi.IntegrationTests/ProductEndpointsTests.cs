using System.Net;
using System.Net.Http.Json;
using EcommerceApi.Application.Common;
using EcommerceApi.Infrastructure.Persistence.Configurations;
using Xunit;

namespace EcommerceApi.IntegrationTests;

public class ProductEndpointsTests : IClassFixture<EcommerceApiFactory>
{
    private readonly HttpClient _client;

    public ProductEndpointsTests(EcommerceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object BuildCreatePayload(string name = "Test Product", decimal price = 99.99m) => new
    {
        name,
        price
    };

    [Fact]
    public async Task Create_product_returns_201_with_correct_data()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/products", BuildCreatePayload("Gaming Chair", 499m));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);
        Assert.Equal("Gaming Chair", product!.Name);
        Assert.Equal(499m, product.Price);
        Assert.NotEqual(Guid.Empty, product.Id);
    }

    [Fact]
    public async Task Create_product_with_invalid_price_returns_400()
    {
        var payload = new { name = "Bad Product", price = 0 };

        var response = await _client.PostAsJsonAsync("/api/v1/products", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_product_with_empty_name_returns_400()
    {
        var payload = new { name = "", price = 10m };

        var response = await _client.PostAsJsonAsync("/api/v1/products", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_seeded_product_returns_200()
    {
        var response = await _client.GetAsync($"/api/v1/products/{SeedIds.Keyboard}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);
        Assert.Equal(SeedIds.Keyboard, product!.Id);
    }

    [Fact]
    public async Task Get_unknown_product_returns_404()
    {
        var response = await _client.GetAsync($"/api/v1/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_returns_seeded_products()
    {
        var paged = await _client.GetFromJsonAsync<PagedResult<ProductResponse>>("/api/v1/products");

        Assert.NotNull(paged);
        Assert.Contains(paged!.Items, p => p.Id == SeedIds.Keyboard);
        Assert.Contains(paged.Items, p => p.Id == SeedIds.Mouse);
        Assert.Contains(paged.Items, p => p.Id == SeedIds.Monitor);
        Assert.True(paged.TotalCount >= 3);
        Assert.Equal(1, paged.Page);
    }

    [Fact]
    public async Task Update_product_returns_200_with_new_values()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/products", BuildCreatePayload("Original", 50m)))
            .Content.ReadFromJsonAsync<ProductResponse>();

        var updatePayload = new { name = "Updated", price = 75m };
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{created!.Id}", updatePayload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.Equal("Updated", updated!.Name);
        Assert.Equal(75m, updated.Price);
    }

    [Fact]
    public async Task Update_unknown_product_returns_404()
    {
        var payload = new { name = "Name", price = 10m };

        var response = await _client.PutAsJsonAsync($"/api/v1/products/{Guid.NewGuid()}", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_product_removes_it()
    {
        var created = await (await _client.PostAsJsonAsync("/api/v1/products", BuildCreatePayload("To Delete", 10m)))
            .Content.ReadFromJsonAsync<ProductResponse>();

        var delete = await _client.DeleteAsync($"/api/v1/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var get = await _client.GetAsync($"/api/v1/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Delete_unknown_product_returns_404()
    {
        var response = await _client.DeleteAsync($"/api/v1/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
