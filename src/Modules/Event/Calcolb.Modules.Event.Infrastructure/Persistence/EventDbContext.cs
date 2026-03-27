using Microsoft.EntityFrameworkCore;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Infrastructure.Persistence;

public sealed class EventDbContext : DbContext
{
    public DbSet<EventEntity> Events => Set<EventEntity>();

    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventDbContext).Assembly);
    }
}
