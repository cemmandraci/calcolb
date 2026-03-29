using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Estimation.Domain.ValueObjects;

public sealed class EstimationSummary : ValueObject
{
    public decimal TotalTransportCost { get; }
    public decimal TotalShoppingCost { get; }
    public decimal TotalExpenseCost { get; }
    public decimal BufferAmount { get; }
    public decimal GrandTotal { get; }
    public decimal CostPerPerson { get; }

    private EstimationSummary(
        decimal totalTransportCost,
        decimal totalShoppingCost,
        decimal totalExpenseCost,
        decimal bufferAmount,
        decimal grandTotal,
        decimal costPerPerson)
    {
        TotalTransportCost = totalTransportCost;
        TotalShoppingCost = totalShoppingCost;
        TotalExpenseCost = totalExpenseCost;
        BufferAmount = bufferAmount;
        GrandTotal = grandTotal;
        CostPerPerson = costPerPerson;
    }

    public static EstimationSummary Create(
        decimal totalTransportCost,
        decimal totalShoppingCost,
        decimal totalExpenseCost,
        decimal bufferAmount,
        int participantCount)
    {
        if (participantCount < 1)
            throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");

        var grandTotal = totalTransportCost + totalShoppingCost + totalExpenseCost + bufferAmount;
        var costPerPerson = grandTotal / participantCount;

        return new EstimationSummary(
            totalTransportCost,
            totalShoppingCost,
            totalExpenseCost,
            bufferAmount,
            grandTotal,
            costPerPerson);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TotalTransportCost;
        yield return TotalShoppingCost;
        yield return TotalExpenseCost;
        yield return BufferAmount;
        yield return GrandTotal;
        yield return CostPerPerson;
    }
}
