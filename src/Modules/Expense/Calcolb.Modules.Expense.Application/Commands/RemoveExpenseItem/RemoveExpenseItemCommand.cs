using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.RemoveExpenseItem;

public sealed record RemoveExpenseItemCommand(
    Guid ExpensePlanId,
    Guid ExpenseItemId) : ICommand<RemoveExpenseItemResult>;

public sealed record RemoveExpenseItemResult(bool Success);
