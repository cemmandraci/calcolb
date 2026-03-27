using Calcolb.Modules.Event.Domain.Events;
using Calcolb.Modules.Event.Domain.ValueObjects;
using Shouldly;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Domain.Tests.Entities;

public class EventTests
{
    [Fact]
    public void Create_WhenValidParameters_ReturnsEvent()
    {
        var eventType = EventType.Piknik;
        var participantCount = ParticipantCount.Create(5);

        var @event = EventEntity.Create(eventType, participantCount);

        @event.ShouldNotBeNull();
        @event.Id.ShouldNotBe(Guid.Empty);
        @event.EventType.ShouldBe(eventType);
        @event.ParticipantCount.ShouldBe(participantCount);
        @event.EventDate.ShouldBeNull();
    }

    [Fact]
    public void Create_WhenEventDateProvided_SetsEventDate()
    {
        var eventType = EventType.Kamp;
        var participantCount = ParticipantCount.Create(10);
        var eventDate = EventDate.Create(new DateTime(2026, 7, 1));

        var @event = EventEntity.Create(eventType, participantCount, eventDate);

        @event.EventDate.ShouldNotBeNull();
        @event.EventDate.ShouldBe(eventDate);
    }

    [Fact]
    public void Create_WhenEventDateIsNull_EventDateIsNull()
    {
        var @event = EventEntity.Create(
            EventType.EvPartisi,
            ParticipantCount.Create(3),
            null);

        @event.EventDate.ShouldBeNull();
    }

    [Fact]
    public void Create_RaisesEventCreatedDomainEvent()
    {
        var eventType = EventType.SporOutdoor;
        var participantCount = ParticipantCount.Create(8);

        var @event = EventEntity.Create(eventType, participantCount);

        @event.DomainEvents.Count.ShouldBe(1);
        var domainEvent = @event.DomainEvents.First().ShouldBeOfType<EventCreatedDomainEvent>();
        domainEvent.CreatedEventId.ShouldBe(@event.Id);
        domainEvent.EventType.ShouldBe(eventType.Value);
        domainEvent.ParticipantCount.ShouldBe(participantCount.Value);
        domainEvent.OccurredOn.ShouldNotBe(default);
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var event1 = EventEntity.Create(EventType.Piknik, ParticipantCount.Create(1));
        var event2 = EventEntity.Create(EventType.Piknik, ParticipantCount.Create(1));

        event1.Id.ShouldNotBe(event2.Id);
    }
}
