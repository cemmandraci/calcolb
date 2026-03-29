using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Estimation.Infrastructure.Persistence.Repositories;

public sealed class EstimationSessionRepository : IEstimationSessionRepository
{
    private readonly EstimationDbContext _context;

    public EstimationSessionRepository(EstimationDbContext context)
    {
        _context = context;
    }

    public async Task<EstimationSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EstimationSessions
            .FirstOrDefaultAsync(es => es.Id == id, cancellationToken);
    }

    public async Task AddAsync(EstimationSession session, CancellationToken cancellationToken = default)
    {
        await _context.EstimationSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EstimationSession session, CancellationToken cancellationToken = default)
    {
        _context.EstimationSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
