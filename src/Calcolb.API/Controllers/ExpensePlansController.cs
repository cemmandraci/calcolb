using Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;
using Calcolb.Modules.Expense.Application.Commands.CalculateExpensePlan;
using Calcolb.Modules.Expense.Application.Commands.CreateExpensePlan;
using Calcolb.Modules.Expense.Application.Commands.RemoveExpenseItem;
using Calcolb.Modules.Expense.Application.Commands.ToggleBuffer;
using Calcolb.Modules.Expense.Application.Queries.GetExpensePlan;
using Calcolb.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/expense-plans")]
[Produces("application/json")]
public sealed class ExpensePlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensePlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni bir gider planı oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateExpensePlanResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateExpensePlan(
        [FromBody] CreateExpensePlanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateExpensePlanCommand(request.EventId), cancellationToken);
        return CreatedAtAction(nameof(GetExpensePlan), new { id = result.ExpensePlanId }, result);
    }

    /// <summary>Gider planına kalem ekler.</summary>
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(AddExpenseItemResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddExpenseItem(
        Guid id,
        [FromBody] AddExpenseItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddExpenseItemCommand(id, request.Name, request.Amount);
        var result = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>Gider planından kalem kaldırır.</summary>
    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveExpenseItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveExpenseItemCommand(id, itemId), cancellationToken);
        return NoContent();
    }

    /// <summary>Buffer (yedek pay) özelliğini açar/kapatır.</summary>
    [HttpPatch("{id:guid}/buffer")]
    [ProducesResponseType(typeof(ToggleBufferResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleBuffer(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ToggleBufferCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Gider planı toplam tutarını hesaplar.</summary>
    [HttpPost("{id:guid}/calculate")]
    [ProducesResponseType(typeof(CalculateExpensePlanResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calculate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CalculateExpensePlanCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Gider planı bilgisini getirir.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetExpensePlanResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpensePlan(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetExpensePlanQuery(id), cancellationToken);
        if (result is null) throw new NotFoundException("Gider planı bulunamadı.");
        return Ok(result);
    }
}

public sealed record CreateExpensePlanRequest(Guid EventId);
public sealed record AddExpenseItemRequest(string Name, decimal Amount);
