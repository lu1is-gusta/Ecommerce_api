using System.Text.Json.Serialization;
using Asp.Versioning;
using EcommerceApi.Api.Endpoints;
using EcommerceApi.Api.Infrastructure.OpenApi;
using EcommerceApi.Api.Middleware;
using EcommerceApi.Application;
using EcommerceApi.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1"));
}

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

// Exposed so WebApplicationFactory<Program> can bootstrap the API in integration tests.
public partial class Program;
