using AwesomeAssertions;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests;

[TestFixture]
public class TypedManaTests
{
    [Test]
    public void CanParse()
    {
        // Arrange - Act - Assert
        var typedMana = TypedMana.Parse("{0}");
        typedMana.Cmc.Should().Be(0);
        typedMana.ToLongString().Should().Be("X:0-G:0-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{1}{C}");
        typedMana.Cmc.Should().Be(2);
        typedMana.ToLongString().Should().Be("X:0-G:1-C:1-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{2}{U}");
        typedMana.Cmc.Should().Be(3);
        typedMana.ToLongString().Should().Be("X:0-G:2-C:0-W:0-U:1-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{B}{B}");
        typedMana.Cmc.Should().Be(2);
        typedMana.ToLongString().Should().Be("X:0-G:0-C:0-W:0-U:0-B:2-R:0-G:0");
        typedMana = TypedMana.Parse("{R}{R}{R}");
        typedMana.Cmc.Should().Be(3);
        typedMana.ToLongString().Should().Be("X:0-G:0-C:0-W:0-U:0-B:0-R:3-G:0");
        typedMana = TypedMana.Parse("{G}{U}{G}{R}");
        typedMana.Cmc.Should().Be(4);
        typedMana.ToLongString().Should().Be("X:0-G:0-C:0-W:0-U:1-B:0-R:1-G:2");
        typedMana = TypedMana.Parse("{X}");
        typedMana.Cmc.Should().Be(0);
        typedMana.ToLongString().Should().Be("X:1-G:0-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{X}{X}");
        typedMana.Cmc.Should().Be(0);
        typedMana.ToLongString().Should().Be("X:2-G:0-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{X}{X}{X}");
        typedMana.Cmc.Should().Be(0);
        typedMana.ToLongString().Should().Be("X:3-G:0-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{X}{X}{5}");
        typedMana.Cmc.Should().Be(5);
        typedMana.ToLongString().Should().Be("X:2-G:5-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana = TypedMana.Parse("{X}{2}{C}{C}{C}{W}{W}{W}{W}{U}{U}{U}{U}{U}{B}{B}{B}{B}{B}{B}{R}{R}{R}{R}{R}{R}{R}{G}{G}{G}{G}{G}{G}{G}{G}");
        typedMana.Cmc.Should().Be(35);
        typedMana.ToLongString().Should().Be("X:1-G:2-C:3-W:4-U:5-B:6-R:7-G:8");
    }

    [Test]
    public void CanAdd()
    {
        // Arrange - Act - Assert
        var typedMana1 = TypedMana.Parse("{1}");
        var typedMana2 = TypedMana.Parse("{1}");
        (typedMana1 + typedMana2).ToLongString().Should().Be("X:0-G:2-C:0-W:0-U:0-B:0-R:0-G:0");
        typedMana1 = TypedMana.Parse("{W}{U}");
        typedMana2 = TypedMana.Parse("{2}{W}{B}{B}");
        (typedMana1 + typedMana2).ToLongString().Should().Be("X:0-G:2-C:0-W:2-U:1-B:2-R:0-G:0");
        typedMana1 = TypedMana.Parse("{C}{C}");
        typedMana2 = TypedMana.Parse("{C}{R}{R}{G}");
        (typedMana1 + typedMana2).ToLongString().Should().Be("X:0-G:0-C:3-W:0-U:0-B:0-R:2-G:1");
    }
}
