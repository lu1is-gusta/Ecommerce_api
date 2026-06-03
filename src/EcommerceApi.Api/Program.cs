using System.Text.Json.Serialization;
using Asp.Versioning;
using EcommerceApi.Api.Endpoints;
using EcommerceApi.Api.Infrastructure.Observability;
using EcommerceApi.Api.Infrastructure.OpenApi;
using EcommerceApi.Api.Middleware;
using EcommerceApi.Application;
using EcommerceApi.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddApplication();

    // In the Testing environment the test host provides its own database (InMemory).
    // We still need the repositories, so we register them without a database provider.
    if (builder.Environment.IsEnvironment("Testing"))
    {
        builder.Services.AddRepositories();
    }
    else
    {
        builder.Services.AddInfrastructure(builder.Configuration);
    }

    builder.Services.AddHealthChecks()
        .AddSqlServer(
            builder.Configuration.GetConnectionString("Default")!,
            name: "sqlserver",
            tags: ["ready"]);

    builder.Services.AddObservability();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });
        options.SchemaFilter<StringEnumSchemaFilter>();
        options.DocInclusionPredicate((docName, apiDesc) =>
        {
            var hasVersionMetadata = apiDesc.ActionDescriptor.EndpointMetadata
                .OfType<ApiVersionMetadata>()
                .Any();

            return hasVersionMetadata
                ? apiDesc.GroupName == docName
                : docName == "v1";
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1"));
    }

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false,
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    })
    .WithTags("Health")
    .WithName("LivenessCheck")
    .WithSummary("Liveness check.")
    .WithDescription("Indicates whether the application process is running. Does not verify external dependencies. Used by orchestrators (e.g. Kubernetes) to decide whether to restart the container.");

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = hc => hc.Tags.Contains("ready"),
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        }
    })
    .WithTags("Health")
    .WithName("ReadinessCheck")
    .WithSummary("Readiness check.")
    .WithDescription("Indicates whether the application is ready to receive traffic. Verifies connectivity with the SQL Server database. Returns 503 if any dependency is unavailable.");

    var versionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    app.MapOrderEndpoints(versionSet);
    app.MapProductEndpoints(versionSet);
    app.MapBuyerEndpoints(versionSet);

    if (!app.Environment.IsEnvironment("Testing"))
    {
        await app.Services.ApplyMigrationsAsync();
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Exposed so WebApplicationFactory<Program> can bootstrap the API in integration tests.
public partial class Program;
