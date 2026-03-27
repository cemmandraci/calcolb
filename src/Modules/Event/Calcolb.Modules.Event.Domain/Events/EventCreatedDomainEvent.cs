using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Event.Domain.Events;

public sealed class EventCreatedDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid CreatedEventId { get; }
    public string EventType { get; }
    public int ParticipantCount { get; }

    public EventCreatedDomainEvent(Guid createdEventId, string eventType, int participantCount)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        CreatedEventId = createdEventId;
        EventType = eventType;
        ParticipantCount = participantCount;
    }
}
