using Calcolb.Modules.Expense.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Queries.GetExpensePlan;

public sealed class GetExpensePlanQueryHandler : IQueryHandler<GetExpensePlanQuery, GetExpensePlanResult?>
{
    private readonly IExpensePlanRepository _repository;

    public GetExpensePlanQueryHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<GetExpensePlanResult?> Handle(
        GetExpensePlanQuery query,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(query.ExpensePlanId, cancellationToken);

        if (plan is null)
            return null;

        var items = plan.ExpenseItems
            .Select(i => new ExpenseItemDto(i.Id, i.Name, i.Amount))
            .ToList();

        return new GetExpensePlanResult(
            plan.Id,
            plan.EventId,
            plan.BufferEnabled,
            plan.BufferPercentage,
            plan.TotalCost,
            items);
    }
}
