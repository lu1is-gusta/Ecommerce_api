using EcommerceApi.Application.Interfaces;
using EcommerceApi.Infrastructure.Persistence;
using EcommerceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApi.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers everything: SQL Server DbContext + repositories.
    /// Call from Program.cs for production/development.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        return services;
    }

    /// <summary>
    /// Registers only the repositories (no database provider).
    /// The caller is responsible for registering a DbContext separately.
    /// Used by the integration-test host to swap in an in-memory provider.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBuyerRepository, BuyerRepository>();
        return services;
    }

    /// <summary>Registers the SQL Server DbContext.</summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' was not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return services;
    }

    /// <summary>Applies pending EF Core migrations (creates the database if needed).</summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}
