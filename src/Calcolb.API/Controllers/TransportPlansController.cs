using Calcolb.Modules.Transport.Application.Commands.AddVehicle;
using Calcolb.Modules.Transport.Application.Commands.CalculateTransportCost;
using Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;
using Calcolb.Modules.Transport.Application.Queries.GetTransportPlan;
using Calcolb.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/transport-plans")]
[Produces("application/json")]
public sealed class TransportPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransportPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni bir ulaşım planı oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTransportPlanResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTransportPlan(
        [FromBody] CreateTransportPlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTransportPlanCommand(
            request.EventId,
            request.TransportType,
            request.ParticipantCount,
            request.CostPerPerson);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTransportPlan), new { id = result.TransportPlanId }, result);
    }

    /// <summary>Ulaşım planına araç ekler.</summary>
    [HttpPost("{id:guid}/vehicles")]
    [ProducesResponseType(typeof(AddVehicleResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddVehicle(
        Guid id,
        [FromBody] AddVehicleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddVehicleCommand(
            id,
            request.PassengerCount,
            request.FuelType,
            request.FuelConsumption,
            request.Origin,
            request.Destination);
        var result = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>Ulaşım maliyetini hesaplar.</summary>
    [HttpPost("{id:guid}/calculate")]
    [ProducesResponseType(typeof(CalculateTransportCostResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calculate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CalculateTransportCostCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Ulaşım planı bilgisini getirir.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetTransportPlanResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransportPlan(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransportPlanQuery(id), cancellationToken);
        if (result is null) throw new NotFoundException("Ulaşım planı bulunamadı.");
        return Ok(result);
    }
}

public sealed record CreateTransportPlanRequest(
    Guid EventId,
    string TransportType,
    int ParticipantCount,
    decimal? CostPerPerson);

public sealed record AddVehicleRequest(
    int PassengerCount,
    string FuelType,
    decimal? FuelConsumption,
    string Origin,
    string Destination);
