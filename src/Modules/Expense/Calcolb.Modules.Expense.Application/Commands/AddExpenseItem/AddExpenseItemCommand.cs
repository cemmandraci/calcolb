using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.AddExpenseItem;

public sealed record AddExpenseItemCommand(
    Guid ExpensePlanId,
    string Name,
    decimal Amount) : ICommand<AddExpenseItemResult>;

public sealed record AddExpenseItemResult(Guid ExpenseItemId);
