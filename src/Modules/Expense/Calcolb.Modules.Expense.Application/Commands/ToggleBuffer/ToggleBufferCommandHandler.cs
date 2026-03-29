using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.ToggleBuffer;

public sealed class ToggleBufferCommandHandler : ICommandHandler<ToggleBufferCommand, ToggleBufferResult>
{
    private readonly IExpensePlanRepository _repository;

    public ToggleBufferCommandHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<ToggleBufferResult> Handle(
        ToggleBufferCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.ExpensePlanId, cancellationToken)
            ?? throw new DomainException("Gider planı bulunamadı.");

        plan.ToggleBuffer();

        await _repository.UpdateAsync(plan, cancellationToken);

        return new ToggleBufferResult(plan.BufferEnabled);
    }
}
