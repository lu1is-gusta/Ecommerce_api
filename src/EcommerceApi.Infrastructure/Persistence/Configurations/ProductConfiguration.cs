using EcommerceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceApi.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // Seed data (mocks). Deterministic ids so they can be referenced right away.
        builder.HasData(
            new { Id = SeedIds.Keyboard, Name = "Mechanical Keyboard", Price = 150.00m },
            new { Id = SeedIds.Mouse, Name = "Wireless Mouse", Price = 80.00m },
            new { Id = SeedIds.Monitor, Name = "27\" Monitor", Price = 1200.00m });
    }
}

/// <summary>Well-known ids for the seeded products, handy for samples and tests.</summary>
public static class SeedIds
{
    public static readonly Guid Keyboard = Guid.Parse("1a1a1a1a-0000-0000-0000-000000000001");
    public static readonly Guid Mouse = Guid.Parse("1a1a1a1a-0000-0000-0000-000000000002");
    public static readonly Guid Monitor = Guid.Parse("1a1a1a1a-0000-0000-0000-000000000003");
}
