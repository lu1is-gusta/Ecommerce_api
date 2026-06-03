using Asp.Versioning;
using Asp.Versioning.Builder;
using EcommerceApi.Api.DTOs;
using EcommerceApi.Application.Common;
using EcommerceApi.Application.UseCases.CreateBuyer;
using EcommerceApi.Application.UseCases.DeleteBuyer;
using EcommerceApi.Application.UseCases.GetBuyers;
using EcommerceApi.Application.UseCases.UpdateBuyer;

namespace EcommerceApi.Api.Endpoints;

public static class BuyerEndpoints
{
    public static IEndpointRouteBuilder MapBuyerEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app
            .MapGroup("/api/v{version:apiVersion}/buyers")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .WithTags("Buyers");

        group.MapPost("/", CreateBuyer)
            .WithName("CreateBuyer")
            .WithSummary("Creates a new buyer.")
            .Produces<BuyerResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", GetBuyers)
            .WithName("GetBuyers")
            .WithSummary("Lists buyers, optionally filtered by name and email.")
            .Produces<IReadOnlyList<BuyerResponse>>();

        group.MapGet("/{id:guid}", GetBuyerById)
            .WithName("GetBuyerById")
            .WithSummary("Gets a single buyer by id.")
            .Produces<BuyerResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateBuyer)
            .WithName("UpdateBuyer")
            .WithSummary("Updates a buyer's name and email.")
            .Produces<BuyerResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteBuyer)
            .WithName("DeleteBuyer")
            .WithSummary("Deletes a buyer (only if the buyer has no associated orders).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return app;
    }

    private static async Task<IResult> CreateBuyer(
        CreateBuyerRequest request,
        CreateBuyerUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Results.Created($"/api/v1/buyers/{result.Id}", result);
    }

    private static async Task<IResult> GetBuyers(
        [AsParameters] BuyerQuery query,
        GetBuyersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(query.ToFilter(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBuyerById(
        Guid id,
        GetBuyerByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateBuyer(
        Guid id,
        UpdateBuyerRequest request,
        UpdateBuyerUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, request, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteBuyer(
        Guid id,
        DeleteBuyerUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
