using Calcolb.Modules.Event.Application.Queries.GetEvent;
using Calcolb.Modules.Event.Application.Repositories;
using Calcolb.Modules.Event.Domain.ValueObjects;
using NSubstitute;
using Shouldly;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Application.Tests.Queries;

public class GetEventQueryHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly GetEventQueryHandler _handler;

    public GetEventQueryHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _handler = new GetEventQueryHandler(_eventRepository);
    }

    [Fact]
    public async Task Handle_WhenEventExists_ReturnsGetEventResult()
    {
        var @event = EventEntity.Create(
            EventType.Piknik,
            ParticipantCount.Create(5),
            EventDate.Create(new DateTime(2026, 6, 15)));

        _eventRepository.GetByIdAsync(@event.Id, Arg.Any<CancellationToken>())
            .Returns(@event);

        var result = await _handler.Handle(new GetEventQuery(@event.Id), CancellationToken.None);

        result.ShouldNotBeNull();
        result.EventId.ShouldBe(@event.Id);
        result.EventType.ShouldBe("Piknik");
        result.ParticipantCount.ShouldBe(5);
        result.EventDate.ShouldBe(new DateTime(2026, 6, 15));
    }

    [Fact]
    public async Task Handle_WhenEventNotFound_ReturnsNull()
    {
        var eventId = Guid.NewGuid();
        _eventRepository.GetByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns((EventEntity?)null);

        var result = await _handler.Handle(new GetEventQuery(eventId), CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_WhenEventHasNoDate_ReturnsNullEventDate()
    {
        var @event = EventEntity.Create(
            EventType.Kamp,
            ParticipantCount.Create(3));

        _eventRepository.GetByIdAsync(@event.Id, Arg.Any<CancellationToken>())
            .Returns(@event);

        var result = await _handler.Handle(new GetEventQuery(@event.Id), CancellationToken.None);

        result.ShouldNotBeNull();
        result.EventDate.ShouldBeNull();
    }
}
