using Calcolb.Modules.Estimation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Estimation.Infrastructure.Persistence;

public sealed class EstimationDbContext : DbContext
{
    public DbSet<EstimationSession> EstimationSessions => Set<EstimationSession>();

    public EstimationDbContext(DbContextOptions<EstimationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EstimationDbContext).Assembly);
    }
}
