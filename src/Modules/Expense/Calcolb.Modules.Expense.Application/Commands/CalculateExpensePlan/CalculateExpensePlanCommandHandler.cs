using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.CalculateExpensePlan;

public sealed class CalculateExpensePlanCommandHandler
    : ICommandHandler<CalculateExpensePlanCommand, CalculateExpensePlanResult>
{
    private readonly IExpensePlanRepository _repository;

    public CalculateExpensePlanCommandHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CalculateExpensePlanResult> Handle(
        CalculateExpensePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.ExpensePlanId, cancellationToken)
            ?? throw new DomainException("Gider planı bulunamadı.");

        var totalCost = plan.Calculate();

        await _repository.UpdateAsync(plan, cancellationToken);

        return new CalculateExpensePlanResult(totalCost, plan.BufferEnabled);
    }
}
