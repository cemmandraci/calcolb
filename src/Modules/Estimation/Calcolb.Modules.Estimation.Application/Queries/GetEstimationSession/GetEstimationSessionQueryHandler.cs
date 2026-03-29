using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Queries.GetEstimationSession;

public sealed class GetEstimationSessionQueryHandler
    : IQueryHandler<GetEstimationSessionQuery, EstimationSessionResult>
{
    private readonly IEstimationSessionRepository _repository;

    public GetEstimationSessionQueryHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<EstimationSessionResult> Handle(
        GetEstimationSessionQuery query,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(query.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        return new EstimationSessionResult(
            session.Id,
            session.EventId,
            session.ParticipantCount,
            session.Status,
            session.CreatedAt,
            session.TransportPlanId,
            session.ShoppingCartId,
            session.ExpensePlanId,
            session.TransportCost,
            session.ShoppingCost,
            session.ExpenseCost);
    }
}
