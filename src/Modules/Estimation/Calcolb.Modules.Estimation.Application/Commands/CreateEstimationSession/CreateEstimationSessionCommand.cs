using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.CreateEstimationSession;

public sealed record CreateEstimationSessionCommand(
    Guid EventId,
    int ParticipantCount) : ICommand<CreateEstimationSessionResult>;

public sealed record CreateEstimationSessionResult(Guid SessionId);
