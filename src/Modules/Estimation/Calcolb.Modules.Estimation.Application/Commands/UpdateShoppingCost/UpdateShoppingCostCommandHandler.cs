using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateShoppingCost;

public sealed class UpdateShoppingCostCommandHandler
    : ICommandHandler<UpdateShoppingCostCommand, UpdateShoppingCostResult>
{
    private readonly IEstimationSessionRepository _repository;

    public UpdateShoppingCostCommandHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<UpdateShoppingCostResult> Handle(
        UpdateShoppingCostCommand command,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(command.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        session.SetShoppingCart(command.ShoppingCartId);
        session.UpdateShoppingCost(command.Cost);

        await _repository.UpdateAsync(session, cancellationToken);

        return new UpdateShoppingCostResult(session.Status == SessionStatus.Completed);
    }
}
