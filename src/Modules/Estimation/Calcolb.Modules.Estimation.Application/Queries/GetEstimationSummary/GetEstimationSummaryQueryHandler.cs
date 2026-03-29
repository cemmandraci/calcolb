using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Estimation.Application.Queries.GetEstimationSummary;

public sealed class GetEstimationSummaryQueryHandler
    : IQueryHandler<GetEstimationSummaryQuery, EstimationSummaryResult>
{
    private readonly IEstimationSessionRepository _repository;

    public GetEstimationSummaryQueryHandler(IEstimationSessionRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<EstimationSummaryResult> Handle(
        GetEstimationSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(query.SessionId, cancellationToken)
            ?? throw new DomainException("Tahmin oturumu bulunamadı.");

        if (session.Summary is null)
            throw new DomainException("Tahmin özeti henüz hesaplanmadı. Tüm adımları tamamlayınız.");

        return new EstimationSummaryResult(
            session.Summary.TotalTransportCost,
            session.Summary.TotalShoppingCost,
            session.Summary.TotalExpenseCost,
            session.Summary.BufferAmount,
            session.Summary.GrandTotal,
            session.Summary.CostPerPerson);
    }
}
