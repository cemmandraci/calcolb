using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Transport.Domain.Events;

public sealed class TransportPlanCalculatedDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid TransportPlanId { get; }
    public Guid RelatedEventId { get; }
    public decimal TotalCost { get; }

    public TransportPlanCalculatedDomainEvent(Guid transportPlanId, Guid relatedEventId, decimal totalCost)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        TransportPlanId = transportPlanId;
        RelatedEventId = relatedEventId;
        TotalCost = totalCost;
    }
}
