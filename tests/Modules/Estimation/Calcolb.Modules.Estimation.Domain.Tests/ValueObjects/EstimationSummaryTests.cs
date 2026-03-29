using Calcolb.Modules.Estimation.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Estimation.Domain.Tests.ValueObjects;

public class EstimationSummaryTests
{
    [Fact]
    public void Create_WithValidData_CalculatesCostPerPerson()
    {
        var summary = EstimationSummary.Create(500m, 1000m, 250m, 0m, 5);

        summary.GrandTotal.ShouldBe(1750m);
        summary.CostPerPerson.ShouldBe(350m);
    }

    [Fact]
    public void Create_WithBufferAmount_IncludesInGrandTotal()
    {
        var summary = EstimationSummary.Create(500m, 1000m, 250m, 100m, 2);

        summary.GrandTotal.ShouldBe(1850m);
        summary.CostPerPerson.ShouldBe(925m);
    }

    [Fact]
    public void Create_WithSingleParticipant_CostPerPersonEqualsGrandTotal()
    {
        var summary = EstimationSummary.Create(200m, 300m, 100m, 0m, 1);

        summary.CostPerPerson.ShouldBe(600m);
        summary.GrandTotal.ShouldBe(600m);
    }

    [Fact]
    public void Create_WithZeroParticipantCount_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            EstimationSummary.Create(500m, 1000m, 250m, 0m, 0));
    }

    [Fact]
    public void Create_WithNegativeParticipantCount_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            EstimationSummary.Create(500m, 1000m, 250m, 0m, -1));
    }

    [Fact]
    public void Create_StoresAllCostsCorrectly()
    {
        var summary = EstimationSummary.Create(100m, 200m, 300m, 50m, 3);

        summary.TotalTransportCost.ShouldBe(100m);
        summary.TotalShoppingCost.ShouldBe(200m);
        summary.TotalExpenseCost.ShouldBe(300m);
        summary.BufferAmount.ShouldBe(50m);
    }

    [Fact]
    public void TwoSummariesWithSameData_AreEqual()
    {
        var summary1 = EstimationSummary.Create(100m, 200m, 300m, 0m, 4);
        var summary2 = EstimationSummary.Create(100m, 200m, 300m, 0m, 4);

        summary1.ShouldBe(summary2);
    }
}
