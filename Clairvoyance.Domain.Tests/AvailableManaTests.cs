using AwesomeAssertions;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests;

[TestFixture]
public class AvailableManaTests
{
    [Test]
    public void CanTellWhetherIsEnoughFor()
    {
        // Arrange - Act - Assert
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{G}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{W}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}{G}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}{W}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}")).Should().BeFalse();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{G}{G}")).Should().BeFalse();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{2}")).Should().BeTrue();
        new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{3}")).Should().BeFalse();

        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}{W}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}{W}{W}")).Should().BeFalse();

        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{R}{G}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{G}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{R}{G}")).Should().BeTrue();
        new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{R}{G}{G}")).Should().BeFalse();

        // TODO bilands...
    }

    [Test]
    public void CanClone()
    {
        // Arrange
        var expectedAvailableMana = new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}");

        // Act
        var actualAvailableMana = expectedAvailableMana.Clone();

        // Assert
        actualAvailableMana.ToString().Should().Be(expectedAvailableMana.ToString());
    }
}
