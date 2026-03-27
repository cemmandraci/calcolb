using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Transport.Infrastructure.Persistence.Repositories;

public sealed class TransportPlanRepository : ITransportPlanRepository
{
    private readonly TransportDbContext _dbContext;

    public TransportPlanRepository(TransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransportPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TransportPlans
            .Include(tp => tp.Vehicles)
                .ThenInclude(v => v.RouteDestination)
            .FirstOrDefaultAsync(tp => tp.Id == id, cancellationToken);
    }

    public async Task AddAsync(TransportPlan transportPlan, CancellationToken cancellationToken = default)
    {
        await _dbContext.TransportPlans.AddAsync(transportPlan, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TransportPlan transportPlan, CancellationToken cancellationToken = default)
    {
        _dbContext.TransportPlans.Update(transportPlan);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
