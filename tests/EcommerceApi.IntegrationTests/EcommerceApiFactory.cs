using EcommerceApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EcommerceApi.IntegrationTests;

/// <summary>
/// Boots the real API pipeline with an isolated in-memory database per test run.
/// Program.cs detects the "Testing" environment and skips SQL Server registration,
/// so this factory only needs to provide the in-memory DbContext and seed data.
/// </summary>
public class EcommerceApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"EcommerceTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Apply the EF model (including HasData product seeds) after the host is built.
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return host;
    }
}
