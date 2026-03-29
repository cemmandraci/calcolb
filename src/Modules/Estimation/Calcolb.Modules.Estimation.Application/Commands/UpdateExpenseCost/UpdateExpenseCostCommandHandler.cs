using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateExpenseCost;

public sealed class UpdateExpenseCostCommandHandler
    : ICommandHandler<UpdateExpenseCostCommand, UpdateExpenseCostResult>
{
    private readonly IEstimationSessionRepository _repository;

    public UpdateExpenseCostCommandHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<UpdateExpenseCostResult> Handle(
        UpdateExpenseCostCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(command.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        session.SetExpensePlan(command.ExpensePlanId);
        session.UpdateExpenseCost(command.Cost);

        await _repository.UpdateAsync(session, cancellationToken);

        return new UpdateExpenseCostResult(session.Status == SessionStatus.Completed);
    }
}
