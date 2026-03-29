using Calcolb.Modules.Estimation.Domain.ValueObjects;
using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Estimation.Domain.Events;

public sealed record SessionCompletedDomainEvent(
    Guid SessionId,
    Guid RelatedEventId,
    EstimationSummary Summary) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
