using Calcolb.Modules.Event.Application.Commands.CreateEvent;
using Calcolb.Modules.Event.Application.Repositories;
using NSubstitute;
using Shouldly;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Application.Tests.Commands;

public class CreateEventCommandHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly CreateEventCommandHandler _handler;

    public CreateEventCommandHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _handler = new CreateEventCommandHandler(_eventRepository);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_ReturnsResultWithEventId()
    {
        var command = new CreateEventCommand("Piknik", 5, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.EventId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenValidCommand_CallsRepositoryAddAsync()
    {
        var command = new CreateEventCommand("Kamp", 10, new DateTime(2026, 7, 1));

        await _handler.Handle(command, CancellationToken.None);

        await _eventRepository.Received(1).AddAsync(
            Arg.Is<EventEntity>(e =>
                e.EventType.Value == "Kamp" &&
                e.ParticipantCount.Value == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEventDateProvided_CreatesEventWithDate()
    {
        var date = new DateTime(2026, 8, 15);
        var command = new CreateEventCommand("DogumGünü", 20, date);

        await _handler.Handle(command, CancellationToken.None);

        await _eventRepository.Received(1).AddAsync(
            Arg.Is<EventEntity>(e => e.EventDate != null && e.EventDate.Value == date),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEventDateIsNull_CreatesEventWithoutDate()
    {
        var command = new CreateEventCommand("EvPartisi", 3, null);

        await _handler.Handle(command, CancellationToken.None);

        await _eventRepository.Received(1).AddAsync(
            Arg.Is<EventEntity>(e => e.EventDate == null),
            Arg.Any<CancellationToken>());
    }
}
