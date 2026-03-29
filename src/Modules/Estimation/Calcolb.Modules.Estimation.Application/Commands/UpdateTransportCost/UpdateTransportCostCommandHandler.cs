using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateTransportCost;

public sealed class UpdateTransportCostCommandHandler
    : ICommandHandler<UpdateTransportCostCommand, UpdateTransportCostResult>
{
    private readonly IEstimationSessionRepository _repository;

    public UpdateTransportCostCommandHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<UpdateTransportCostResult> Handle(
        UpdateTransportCostCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(command.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        session.SetTransportPlan(command.TransportPlanId);
        session.UpdateTransportCost(command.Cost);

        await _repository.UpdateAsync(session, cancellationToken);

        return new UpdateTransportCostResult(session.Status == SessionStatus.Completed);
    }
}
