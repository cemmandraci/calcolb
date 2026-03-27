using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.ValueObjects;

public class FuelTypeTests
{
    [Theory]
    [InlineData("Benzin")]
    [InlineData("Dizel")]
    [InlineData("LPG")]
    public void FromString_WhenValidFuelType_ReturnsFuelType(string value)
    {
        var fuelType = FuelType.FromString(value);

        fuelType.ShouldNotBeNull();
        fuelType.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData("benzin")]
    [InlineData("DIZEL")]
    [InlineData("lpg")]
    public void FromString_WhenCaseInsensitive_ReturnsFuelType(string value)
    {
        var fuelType = FuelType.FromString(value);

        fuelType.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Elektrik")]
    [InlineData("Hidrojen")]
    public void FromString_WhenInvalidFuelType_ThrowsDomainException(string value)
    {
        Should.Throw<DomainException>(() => FuelType.FromString(value));
    }

    [Fact]
    public void GetAll_ReturnsAllThreeFuelTypes()
    {
        var allTypes = FuelType.GetAll();

        allTypes.Count.ShouldBe(3);
    }

    [Fact]
    public void StaticInstances_HaveCorrectValues()
    {
        FuelType.Benzin.Value.ShouldBe("Benzin");
        FuelType.Dizel.Value.ShouldBe("Dizel");
        FuelType.LPG.Value.ShouldBe("LPG");
    }
}
