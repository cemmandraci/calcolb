using Calcolb.Modules.Event.Application.Commands.CreateEvent;
using Calcolb.Modules.Event.Application.Queries.GetEvent;
using Calcolb.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
public sealed class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni bir etkinlik oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateEventResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateEvent(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand(request.EventType, request.ParticipantCount, request.EventDate);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEvent), new { id = result.EventId }, result);
    }

    /// <summary>Etkinlik bilgisini getirir.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetEventResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEventQuery(id), cancellationToken);
        if (result is null) throw new NotFoundException("Etkinlik bulunamadı.");
        return Ok(result);
    }
}

public sealed record CreateEventRequest(string EventType, int ParticipantCount, DateTime? EventDate);
