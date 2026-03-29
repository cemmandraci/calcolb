using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.CompleteEstimationSession;

public sealed record CompleteEstimationSessionCommand(Guid SessionId)
    : ICommand<CompleteEstimationSessionResult>;

public sealed record CompleteEstimationSessionResult(Guid SessionId);
