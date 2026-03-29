using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.RemoveExpenseItem;

public sealed class RemoveExpenseItemCommandHandler
    : ICommandHandler<RemoveExpenseItemCommand, RemoveExpenseItemResult>
{
    private readonly IExpensePlanRepository _repository;

    public RemoveExpenseItemCommandHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<RemoveExpenseItemResult> Handle(
        RemoveExpenseItemCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.ExpensePlanId, cancellationToken)
            ?? throw new DomainException("Gider planı bulunamadı.");

        plan.RemoveItem(command.ExpenseItemId);

        await _repository.UpdateAsync(plan, cancellationToken);

        return new RemoveExpenseItemResult(true);
    }
}
