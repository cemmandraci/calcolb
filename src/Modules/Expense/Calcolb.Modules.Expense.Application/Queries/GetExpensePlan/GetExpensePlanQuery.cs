using Mediator;

namespace Calcolb.Modules.Expense.Application.Queries.GetExpensePlan;

public sealed record GetExpensePlanQuery(Guid ExpensePlanId) : IQuery<GetExpensePlanResult?>;

public sealed record GetExpensePlanResult(
    Guid ExpensePlanId,
    Guid EventId,
    bool BufferEnabled,
    decimal BufferPercentage,
    decimal TotalCost,
    List<ExpenseItemDto> ExpenseItems);

public sealed record ExpenseItemDto(
    Guid ExpenseItemId,
    string Name,
    decimal Amount);
