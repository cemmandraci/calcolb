using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;

public sealed class AddExpenseItemCommandHandler
    : ICommandHandler<AddExpenseItemCommand, AddExpenseItemResult>
{
    private readonly IExpensePlanRepository _repository;

    public AddExpenseItemCommandHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<AddExpenseItemResult> Handle(
        AddExpenseItemCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.ExpensePlanId, cancellationToken)
            ?? throw new DomainException("Gider planı bulunamadı.");

        var item = plan.AddItem(command.Name, command.Amount);

        await _repository.UpdateAsync(plan, cancellationToken);

        return new AddExpenseItemResult(item.Id);
    }
}
