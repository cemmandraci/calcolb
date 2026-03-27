using Calcolb.Modules.Transport.Domain.Entities;

namespace Calcolb.Modules.Transport.Application.Repositories;

public interface ITransportPlanRepository
{
    Task<TransportPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TransportPlan transportPlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(TransportPlan transportPlan, CancellationToken cancellationToken = default);
}
