using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.ValueObjects;

public class TransportTypeTests
{
    [Theory]
    [InlineData("Arabayla")]
    [InlineData("TopluTasima")]
    [InlineData("Yuruyerek")]
    public void FromString_WhenValidTransportType_ReturnsTransportType(string value)
    {
        var transportType = TransportType.FromString(value);

        transportType.ShouldNotBeNull();
        transportType.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData("arabayla")]
    [InlineData("TOPLUTASIMA")]
    [InlineData("yuruyerek")]
    public void FromString_WhenCaseInsensitive_ReturnsTransportType(string value)
    {
        var transportType = TransportType.FromString(value);

        transportType.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Bisiklet")]
    [InlineData("Ucak")]
    public void FromString_WhenInvalidTransportType_ThrowsDomainException(string value)
    {
        Should.Throw<DomainException>(() => TransportType.FromString(value));
    }

    [Fact]
    public void GetAll_ReturnsAllThreeTransportTypes()
    {
        var allTypes = TransportType.GetAll();

        allTypes.Count.ShouldBe(3);
    }

    [Fact]
    public void Equals_WhenSameValue_ReturnsTrue()
    {
        var type1 = TransportType.FromString("Arabayla");
        var type2 = TransportType.FromString("Arabayla");

        type1.ShouldBe(type2);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ReturnsFalse()
    {
        var type1 = TransportType.FromString("Arabayla");
        var type2 = TransportType.FromString("Yuruyerek");

        type1.ShouldNotBe(type2);
    }

    [Fact]
    public void StaticInstances_HaveCorrectValues()
    {
        TransportType.Arabayla.Value.ShouldBe("Arabayla");
        TransportType.TopluTasima.Value.ShouldBe("TopluTasima");
        TransportType.Yuruyerek.Value.ShouldBe("Yuruyerek");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var transportType = TransportType.FromString("Arabayla");

        transportType.ToString().ShouldBe("Arabayla");
    }
}
