using Mediator;

namespace Calcolb.Modules.Event.Application.Commands.CreateEvent;

public sealed record CreateEventCommand(
    string EventType,
    int ParticipantCount,
    DateTime? EventDate) : ICommand<CreateEventResult>;

public sealed record CreateEventResult(Guid EventId);
