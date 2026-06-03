using Asp.Versioning;
using Asp.Versioning.Builder;
using EcommerceApi.Api.DTOs;
using EcommerceApi.Application.Common;
using EcommerceApi.Application.UseCases.CancelOrder;
using EcommerceApi.Application.UseCases.CreateOrder;
using EcommerceApi.Application.UseCases.DeleteOrder;
using EcommerceApi.Application.UseCases.GetOrders;
using EcommerceApi.Application.UseCases.UpdateOrder;

namespace EcommerceApi.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
    {
        var group = app
            .MapGroup("/api/v{version:apiVersion}/orders")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .WithTags("Orders");

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithSummary("Creates a new order.")
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", GetOrders)
            .WithName("GetOrders")
            .WithSummary("Lists orders, optionally filtered by status, buyer and creation date range. Supports pagination via page and pageSize.")
            .Produces<PagedResult<OrderResponse>>();

        group.MapGet("/{id:guid}", GetOrderById)
            .WithName("GetOrderById")
            .WithSummary("Gets a single order by id.")
            .Produces<OrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateOrder)
            .WithName("UpdateOrder")
            .WithSummary("Updates the items of an order (only while it has not been processed).")
            .Produces<OrderResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapPut("/{id:guid}/cancel", CancelOrder)
            .WithName("CancelOrder")
            .WithSummary("Cancels an order (only while started or processed).")
            .Produces<OrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/{id:guid}", DeleteOrder)
            .WithName("DeleteOrder")
            .WithSummary("Deletes an order.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        CreateOrderUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);
        return Results.Created($"/api/v1/orders/{result.Id}", result);
    }

    private static async Task<IResult> GetOrders(
        [AsParameters] OrderQuery query,
        GetOrdersUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(query.ToFilter(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        GetOrderByIdUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateOrder(
        Guid id,
        UpdateOrderRequest request,
        UpdateOrderUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, request, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        CancelOrderUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteOrder(
        Guid id,
        DeleteOrderUseCase useCase,
        CancellationToken cancellationToken)
    {
        await useCase.ExecuteAsync(id, cancellationToken);
        return Results.NoContent();
    }
}
