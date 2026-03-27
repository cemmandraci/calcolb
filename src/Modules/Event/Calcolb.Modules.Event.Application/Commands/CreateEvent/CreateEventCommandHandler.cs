using Calcolb.Modules.Event.Application.Repositories;
using Calcolb.Modules.Event.Domain.ValueObjects;
using Mediator;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Application.Commands.CreateEvent;

public sealed class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IEventRepository _eventRepository;

    public CreateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async ValueTask<CreateEventResult> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var eventType = EventType.FromString(command.EventType);
        var participantCount = ParticipantCount.Create(command.ParticipantCount);
        var eventDate = EventDate.Create(command.EventDate);

        var @event = EventEntity.Create(eventType, participantCount, eventDate);

        await _eventRepository.AddAsync(@event, cancellationToken);

        return new CreateEventResult(@event.Id);
    }
}
