using Asp.Versioning;
using Asp.Versioning.Builder;
using EcommerceApi.Api.DTOs;
using EcommerceApi.Application.Common;
using EcommerceApi.Application.UseCases.CreateProduct;
using EcommerceApi.Application.UseCases.DeleteProduct;
using EcommerceApi.Application.UseCases.GetProducts;
using EcommerceApi.Application.UseCases.UpdateProduct;

namespace EcommerceApi.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app
            .MapGroup("/api/v{version:apiVersion}/products")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .WithTags("Products");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Creates a new product.")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", GetProducts)
            .WithName("GetProducts")
            .WithSummary("Lists products, optionally filtered by name and price range.")
            .Produces<IReadOnlyList<ProductResponse>>();

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Gets a single product by id.")
            .Produces<ProductResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Updates a product's name and price.")
            .Produces<ProductResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Deletes a product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateProduct(
        CreateProductRequest request,
        CreateProductUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Results.Created($"/api/v1/products/{result.Id}", result);
    }

    private static async Task<IResult> GetProducts(
        [AsParameters] ProductQuery query,
        GetProductsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(query.ToFilter(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        GetProductByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        UpdateProductUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, request, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteProduct(
        Guid id,
        DeleteProductUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
