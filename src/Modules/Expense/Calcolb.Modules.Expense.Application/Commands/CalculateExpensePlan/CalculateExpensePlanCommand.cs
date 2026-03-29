using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.CalculateExpensePlan;

public sealed record CalculateExpensePlanCommand(Guid ExpensePlanId) : ICommand<CalculateExpensePlanResult>;

public sealed record CalculateExpensePlanResult(decimal TotalCost, bool BufferApplied);
