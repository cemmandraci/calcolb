using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Application.Repositories;

public interface IEventRepository
{
    Task<EventEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(EventEntity @event, CancellationToken cancellationToken = default);
}
