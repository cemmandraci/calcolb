using Calcolb.Modules.Estimation.Domain.Entities;

namespace Calcolb.Modules.Estimation.Application.Repositories;

public interface IEstimationSessionRepository
{
    Task<EstimationSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(EstimationSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(EstimationSession session, CancellationToken cancellationToken = default);
}
