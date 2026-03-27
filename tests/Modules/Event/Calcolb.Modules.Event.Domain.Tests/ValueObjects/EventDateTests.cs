using Calcolb.Modules.Event.Domain.ValueObjects;
using Shouldly;

namespace Calcolb.Modules.Event.Domain.Tests.ValueObjects;

public class EventDateTests
{
    [Fact]
    public void Create_WhenValueIsNull_ReturnsNull()
    {
        var result = EventDate.Create(null);

        result.ShouldBeNull();
    }

    [Fact]
    public void Create_WhenValueIsProvided_ReturnsEventDate()
    {
        var date = new DateTime(2026, 6, 15);

        var result = EventDate.Create(date);

        result.ShouldNotBeNull();
        result.Value.ShouldBe(date);
    }

    [Fact]
    public void Equals_WhenSameDate_ReturnsTrue()
    {
        var date = new DateTime(2026, 6, 15);
        var date1 = EventDate.Create(date);
        var date2 = EventDate.Create(date);

        date1.ShouldBe(date2);
    }

    [Fact]
    public void Equals_WhenDifferentDate_ReturnsFalse()
    {
        var date1 = EventDate.Create(new DateTime(2026, 6, 15));
        var date2 = EventDate.Create(new DateTime(2026, 7, 20));

        date1.ShouldNotBe(date2);
    }

    [Fact]
    public void ToString_ReturnsFormattedDate()
    {
        var result = EventDate.Create(new DateTime(2026, 6, 15));

        result!.ToString().ShouldBe("2026-06-15");
    }
}
