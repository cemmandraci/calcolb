using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("shopping_products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(p => p.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.MarketType)
            .HasColumnName("market_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Ignore(p => p.DomainEvents);
    }
}
