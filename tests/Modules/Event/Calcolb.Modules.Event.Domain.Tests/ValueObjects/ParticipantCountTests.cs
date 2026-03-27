using Calcolb.Modules.Event.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Event.Domain.Tests.ValueObjects;

public class ParticipantCountTests
{
    [Fact]
    public void Create_WhenValueIsOne_ReturnsParticipantCount()
    {
        var count = ParticipantCount.Create(1);

        count.ShouldNotBeNull();
        count.Value.ShouldBe(1);
    }

    [Fact]
    public void Create_WhenValueIsGreaterThanOne_ReturnsParticipantCount()
    {
        var count = ParticipantCount.Create(50);

        count.Value.ShouldBe(50);
    }

    [Fact]
    public void Create_WhenValueIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => ParticipantCount.Create(0))
            .Message.ShouldContain("en az 1");
    }

    [Fact]
    public void Create_WhenValueIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() => ParticipantCount.Create(-5))
            .Message.ShouldContain("en az 1");
    }

    [Fact]
    public void Equals_WhenSameValue_ReturnsTrue()
    {
        var count1 = ParticipantCount.Create(10);
        var count2 = ParticipantCount.Create(10);

        count1.ShouldBe(count2);
    }

    [Fact]
    public void Equals_WhenDifferentValue_ReturnsFalse()
    {
        var count1 = ParticipantCount.Create(10);
        var count2 = ParticipantCount.Create(20);

        count1.ShouldNotBe(count2);
    }

    [Fact]
    public void ToString_ReturnsStringValue()
    {
        var count = ParticipantCount.Create(7);

        count.ToString().ShouldBe("7");
    }
}
