using Calcolb.Modules.Estimation.Domain.Enums;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Queries.GetEstimationSession;

public sealed record GetEstimationSessionQuery(Guid SessionId)
    : IQuery<EstimationSessionResult?>;

public sealed record EstimationSessionResult(
    Guid SessionId,
    Guid EventId,
    int ParticipantCount,
    SessionStatus Status,
    DateTime CreatedAt,
    Guid? TransportPlanId,
    Guid? ShoppingCartId,
    Guid? ExpensePlanId,
    decimal? TransportCost,
    decimal? ShoppingCost,
    decimal? ExpenseCost);
