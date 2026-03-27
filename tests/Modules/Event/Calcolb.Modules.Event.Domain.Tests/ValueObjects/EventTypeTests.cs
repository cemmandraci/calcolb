using Calcolb.Modules.Event.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Event.Domain.Tests.ValueObjects;

public class EventTypeTests
{
    [Theory]
    [InlineData("Piknik")]
    [InlineData("Kamp")]
    [InlineData("DogumGünü")]
    [InlineData("EvPartisi")]
    [InlineData("SporOutdoor")]
    public void FromString_WhenValidEventType_ReturnsEventType(string value)
    {
        var eventType = EventType.FromString(value);

        eventType.ShouldNotBeNull();
        eventType.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData("piknik")]
    [InlineData("KAMP")]
    [InlineData("evpartisi")]
    public void FromString_WhenCaseInsensitive_ReturnsEventType(string value)
    {
        var eventType = EventType.FromString(value);

        eventType.ShouldNotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("GeçersizTip")]
    [InlineData("Parti")]
    public void FromString_WhenInvalidEventType_ThrowsDomainException(string value)
    {
        Should.Throw<DomainException>(() => EventType.FromString(value));
    }

    [Fact]
    public void GetAll_ReturnsAllFiveEventTypes()
    {
        var allTypes = EventType.GetAll();

        allTypes.Count.ShouldBe(5);
    }

    [Fact]
    public void Equals_WhenSameValue_ReturnsTrue()
    {
        var type1 = EventType.FromString("Piknik");
        var type2 = EventType.FromString("Piknik");

        type1.ShouldBe(type2);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ReturnsFalse()
    {
        var type1 = EventType.FromString("Piknik");
        var type2 = EventType.FromString("Kamp");

        type1.ShouldNotBe(type2);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var eventType = EventType.FromString("Piknik");

        eventType.ToString().ShouldBe("Piknik");
    }

    [Fact]
    public void StaticInstances_HaveCorrectValues()
    {
        EventType.Piknik.Value.ShouldBe("Piknik");
        EventType.Kamp.Value.ShouldBe("Kamp");
        EventType.DogumGünü.Value.ShouldBe("DogumGünü");
        EventType.EvPartisi.Value.ShouldBe("EvPartisi");
        EventType.SporOutdoor.Value.ShouldBe("SporOutdoor");
    }
}
