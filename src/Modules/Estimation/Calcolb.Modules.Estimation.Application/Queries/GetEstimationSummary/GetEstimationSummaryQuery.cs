using Mediator;

namespace Calcolb.Modules.Estimation.Application.Queries.GetEstimationSummary;

public sealed record GetEstimationSummaryQuery(Guid SessionId)
    : IQuery<EstimationSummaryResult>;

public sealed record EstimationSummaryResult(
    decimal TotalTransportCost,
    decimal TotalShoppingCost,
    decimal TotalExpenseCost,
    decimal BufferAmount,
    decimal GrandTotal,
    decimal CostPerPerson);
