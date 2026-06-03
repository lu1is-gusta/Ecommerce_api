using OpenTelemetry.Trace;

namespace EcommerceApi.Api.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter());

        return services;
    }
}
