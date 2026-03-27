using Calcolb.Modules.Event.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Infrastructure.Persistence.Repositories;

public sealed class EventRepository : IEventRepository
{
    private readonly EventDbContext _dbContext;

    public EventRepository(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EventEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(EventEntity @event, CancellationToken cancellationToken = default)
    {
        await _dbContext.Events.AddAsync(@event, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
