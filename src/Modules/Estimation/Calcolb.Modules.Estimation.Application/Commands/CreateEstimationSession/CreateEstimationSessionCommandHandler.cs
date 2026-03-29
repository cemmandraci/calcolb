using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.CreateEstimationSession;

public sealed class CreateEstimationSessionCommandHandler
    : ICommandHandler<CreateEstimationSessionCommand, CreateEstimationSessionResult>
{
    private readonly IEstimationSessionRepository _repository;

    public CreateEstimationSessionCommandHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CreateEstimationSessionResult> Handle(
        CreateEstimationSessionCommand command,
        CancellationToken cancellationToken)
    {
        var session = EstimationSession.Create(command.EventId, command.ParticipantCount);

        await _repository.AddAsync(session, cancellationToken);

        return new CreateEstimationSessionResult(session.Id);
    }
}
