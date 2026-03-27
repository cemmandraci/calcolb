using Calcolb.Modules.Event.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Event.Application.Queries.GetEvent;

public sealed class GetEventQueryHandler : IQueryHandler<GetEventQuery, GetEventResult?>
{
    private readonly IEventRepository _eventRepository;

    public GetEventQueryHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async ValueTask<GetEventResult?> Handle(GetEventQuery query, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetByIdAsync(query.EventId, cancellationToken);

        if (@event is null)
            return null;

        return new GetEventResult(
            @event.Id,
            @event.EventType.Value,
            @event.ParticipantCount.Value,
            @event.EventDate?.Value);
    }
}
