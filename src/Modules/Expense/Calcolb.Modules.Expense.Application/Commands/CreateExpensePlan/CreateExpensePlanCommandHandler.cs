using Calcolb.Modules.Expense.Application.Repositories;
using Calcolb.Modules.Expense.Domain.Entities;
using Mediator;

namespace Calcolb.Modules.Expense.Application.Commands.CreateExpensePlan;

public sealed class CreateExpensePlanCommandHandler
    : ICommandHandler<CreateExpensePlanCommand, CreateExpensePlanResult>
{
    private readonly IExpensePlanRepository _repository;

    public CreateExpensePlanCommandHandler(IExpensePlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CreateExpensePlanResult> Handle(
        CreateExpensePlanCommand command,
        CancellationToken cancellationToken)
    {
        var plan = ExpensePlan.Create(command.EventId);

        await _repository.AddAsync(plan, cancellationToken);

        return new CreateExpensePlanResult(plan.Id);
    }
}
