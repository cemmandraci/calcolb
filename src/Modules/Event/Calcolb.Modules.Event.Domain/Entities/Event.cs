using Calcolb.Modules.Event.Domain.Events;
using Calcolb.Modules.Event.Domain.ValueObjects;
using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Event.Domain.Entities;

public sealed class Event : AggregateRoot<Guid>
{
    public EventType EventType { get; private set; } = null!;
    public ParticipantCount ParticipantCount { get; private set; } = null!;
    public EventDate? EventDate { get; private set; }

    private Event() { }

    public static Event Create(EventType eventType, ParticipantCount participantCount, EventDate? eventDate = null)
    {
        var @event = new Event
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            ParticipantCount = participantCount,
            EventDate = eventDate
        };

        @event.RaiseDomainEvent(new EventCreatedDomainEvent(
            @event.Id,
            eventType.Value,
            participantCount.Value));

        return @event;
    }
}
