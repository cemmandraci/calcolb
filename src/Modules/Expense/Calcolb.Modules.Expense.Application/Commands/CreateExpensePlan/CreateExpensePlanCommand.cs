using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.CreateExpensePlan;

public sealed record CreateExpensePlanCommand(Guid EventId) : ICommand<CreateExpensePlanResult>;

public sealed record CreateExpensePlanResult(Guid ExpensePlanId);
