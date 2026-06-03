using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EcommerceApi.Api.Infrastructure.OpenApi;

/// <summary>
/// Adds the health check endpoints to the Swagger document. Endpoints mapped via
/// <c>MapHealthChecks</c> are not exposed to the ApiExplorer, so Swashbuckle cannot
/// discover them automatically. This filter injects their paths manually.
/// </summary>
public class HealthChecksDocumentFilter : IDocumentFilter
{
    private static readonly OpenApiTag HealthTag = new() { Name = "Health" };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        AddHealthPath(
            swaggerDoc,
            "/health/live",
            operationId: "LivenessCheck",
            summary: "Liveness check.",
            description: "Indicates whether the application process is running. Does not verify external dependencies. Used by orchestrators (e.g. Kubernetes) to decide whether to restart the container.");

        AddHealthPath(
            swaggerDoc,
            "/health/ready",
            operationId: "ReadinessCheck",
            summary: "Readiness check.",
            description: "Indicates whether the application is ready to receive traffic. Verifies connectivity with the SQL Server database. Returns 503 if any dependency is unavailable.");
    }

    private static void AddHealthPath(
        OpenApiDocument swaggerDoc,
        string path,
        string operationId,
        string summary,
        string description)
    {
        var operation = new OpenApiOperation
        {
            Tags = [HealthTag],
            OperationId = operationId,
            Summary = summary,
            Description = description,
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse { Description = "The service is healthy." },
                ["503"] = new OpenApiResponse { Description = "The service is unavailable." }
            }
        };

        swaggerDoc.Paths.Add(path, new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                [OperationType.Get] = operation
            }
        });
    }
}
