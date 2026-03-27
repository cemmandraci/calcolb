using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.ValueObjects;

public class PublicTransportCostTests
{
    [Fact]
    public void Create_WhenValidParameters_ReturnsPublicTransportCost()
    {
        var cost = PublicTransportCost.Create(15.50m, 5);

        cost.ShouldNotBeNull();
        cost.CostPerPerson.ShouldBe(15.50m);
        cost.ParticipantCount.ShouldBe(5);
        cost.TotalAmount.ShouldBe(77.50m);
    }

    [Fact]
    public void Create_WhenCostPerPersonIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => PublicTransportCost.Create(0, 5));
    }

    [Fact]
    public void Create_WhenCostPerPersonIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => PublicTransportCost.Create(-10m, 5));
    }

    [Fact]
    public void Create_WhenParticipantCountIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => PublicTransportCost.Create(15m, 0));
    }

    [Fact]
    public void TotalAmount_IsCalculatedCorrectly()
    {
        var cost = PublicTransportCost.Create(20m, 3);

        cost.TotalAmount.ShouldBe(60m);
    }

    [Fact]
    public void Equals_WhenSameValues_ReturnsTrue()
    {
        var cost1 = PublicTransportCost.Create(10m, 4);
        var cost2 = PublicTransportCost.Create(10m, 4);

        cost1.ShouldBe(cost2);
    }

    [Fact]
    public void Equals_WhenDifferentValues_ReturnsFalse()
    {
        var cost1 = PublicTransportCost.Create(10m, 4);
        var cost2 = PublicTransportCost.Create(15m, 4);

        cost1.ShouldNotBe(cost2);
    }
}
