using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("shopping_cart_items");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ci => ci.ShoppingCartId)
            .HasColumnName("shopping_cart_id")
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(10,3)")
            .IsRequired();

        builder.Property(ci => ci.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ci => ci.MarketType)
            .HasColumnName("market_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ci => ci.TotalPrice)
            .HasColumnName("total_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
