using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.CompleteEstimationSession;

public sealed class CompleteEstimationSessionCommandHandler
    : ICommandHandler<CompleteEstimationSessionCommand, CompleteEstimationSessionResult>
{
    private readonly IEstimationSessionRepository _repository;

    public CompleteEstimationSessionCommandHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CompleteEstimationSessionResult> Handle(
        CompleteEstimationSessionCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(command.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        session.Complete();

        await _repository.UpdateAsync(session, cancellationToken);

        return new CompleteEstimationSessionResult(session.Id);
    }
}
