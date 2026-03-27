using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.ValueObjects;

public class FuelConsumptionTests
{
    [Fact]
    public void Create_WhenNoValueProvided_ReturnsDefaultValue()
    {
        var consumption = FuelConsumption.Create();

        consumption.Value.ShouldBe(FuelConsumption.DefaultValue);
        consumption.Value.ShouldBe(8m);
    }

    [Fact]
    public void Create_WhenNullProvided_ReturnsDefaultValue()
    {
        var consumption = FuelConsumption.Create(null);

        consumption.Value.ShouldBe(8m);
    }

    [Fact]
    public void Create_WhenCustomValueProvided_ReturnsCustomValue()
    {
        var consumption = FuelConsumption.Create(6.5m);

        consumption.Value.ShouldBe(6.5m);
    }

    [Fact]
    public void Create_WhenValueIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => FuelConsumption.Create(0));
    }

    [Fact]
    public void Create_WhenValueIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => FuelConsumption.Create(-5m));
    }

    [Fact]
    public void Equals_WhenSameValue_ReturnsTrue()
    {
        var c1 = FuelConsumption.Create(10m);
        var c2 = FuelConsumption.Create(10m);

        c1.ShouldBe(c2);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ReturnsFalse()
    {
        var c1 = FuelConsumption.Create(8m);
        var c2 = FuelConsumption.Create(12m);

        c1.ShouldNotBe(c2);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var consumption = FuelConsumption.Create(8m);

        consumption.ToString().ShouldBe("8 lt/100km");
    }
}
