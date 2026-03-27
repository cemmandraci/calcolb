using Calcolb.Modules.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Transport.Infrastructure.Persistence;

public sealed class TransportDbContext : DbContext
{
    public DbSet<TransportPlan> TransportPlans => Set<TransportPlan>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<RouteDestination> RouteDestinations => Set<RouteDestination>();

    public TransportDbContext(DbContextOptions<TransportDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransportDbContext).Assembly);
    }
}
