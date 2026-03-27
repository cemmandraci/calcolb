using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence;

public sealed class ShoppingDbContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingDbContext).Assembly);
    }
}
