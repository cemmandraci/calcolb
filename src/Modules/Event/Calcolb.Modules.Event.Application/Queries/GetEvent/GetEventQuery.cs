using Mediator;

namespace Calcolb.Modules.Event.Application.Queries.GetEvent;

public sealed record GetEventQuery(Guid EventId) : IQuery<GetEventResult?>;

public sealed record GetEventResult(
    Guid EventId,
    string EventType,
    int ParticipantCount,
    DateTime? EventDate);
