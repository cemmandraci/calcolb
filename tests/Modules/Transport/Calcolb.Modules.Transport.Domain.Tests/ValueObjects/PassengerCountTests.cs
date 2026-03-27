using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.ValueObjects;

public class PassengerCountTests
{
    [Fact]
    public void Create_WhenValueIsOne_ReturnsPassengerCount()
    {
        var count = PassengerCount.Create(1);

        count.ShouldNotBeNull();
        count.Value.ShouldBe(1);
    }

    [Fact]
    public void Create_WhenValueIsGreaterThanOne_ReturnsPassengerCount()
    {
        var count = PassengerCount.Create(4);

        count.Value.ShouldBe(4);
    }

    [Fact]
    public void Create_WhenValueIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => PassengerCount.Create(0))
            .Message.ShouldContain("en az 1");
    }

    [Fact]
    public void Create_WhenValueIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => PassengerCount.Create(-3))
            .Message.ShouldContain("en az 1");
    }

    [Fact]
    public void Equals_WhenSameValue_ReturnsTrue()
    {
        var count1 = PassengerCount.Create(2);
        var count2 = PassengerCount.Create(2);

        count1.ShouldBe(count2);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ReturnsFalse()
    {
        var count1 = PassengerCount.Create(2);
        var count2 = PassengerCount.Create(5);

        count1.ShouldNotBe(count2);
    }

    [Fact]
    public void ToString_ReturnsStringValue()
    {
        var count = PassengerCount.Create(3);

        count.ToString().ShouldBe("3");
    }
}
