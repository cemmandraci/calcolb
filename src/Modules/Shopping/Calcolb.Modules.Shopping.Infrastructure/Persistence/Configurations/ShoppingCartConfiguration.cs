using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence.Configurations;

public sealed class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("shopping_carts");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(sc => sc.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(sc => sc.TotalCost)
            .HasColumnName("total_cost")
            .HasColumnType("decimal(18,2)");

        builder.HasMany(sc => sc.CartItems)
            .WithOne()
            .HasForeignKey(ci => ci.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(sc => sc.DomainEvents);

        builder.Metadata
            .FindNavigation(nameof(ShoppingCart.CartItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
