using Calcolb.Modules.Estimation.Application.Commands.CompleteEstimationSession;
using Calcolb.Modules.Estimation.Application.Commands.CreateEstimationSession;
using Calcolb.Modules.Estimation.Application.Queries.GetEstimationSession;
using Calcolb.Modules.Estimation.Application.Queries.GetEstimationSummary;
using Calcolb.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/estimation-sessions")]
[Produces("application/json")]
public sealed class EstimationSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstimationSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni bir tahmin oturumu oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateEstimationSessionResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEstimationSession(
        [FromBody] CreateEstimationSessionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEstimationSessionCommand(request.EventId, request.ParticipantCount);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEstimationSession), new { id = result.SessionId }, result);
    }

    /// <summary>Tahmin oturumunu tamamlar.</summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(CompleteEstimationSessionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteEstimationSession(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteEstimationSessionCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Tahmin oturumu bilgisini getirir.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstimationSessionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEstimationSession(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEstimationSessionQuery(id), cancellationToken);
        if (result is null) throw new NotFoundException("Tahmin oturumu bulunamadı.");
        return Ok(result);
    }

    /// <summary>Tahmin özetini getirir.</summary>
    [HttpGet("{id:guid}/summary")]
    [ProducesResponseType(typeof(EstimationSummaryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEstimationSummary(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEstimationSummaryQuery(id), cancellationToken);
        return Ok(result);
    }
}

public sealed record CreateEstimationSessionRequest(Guid EventId, int ParticipantCount);
