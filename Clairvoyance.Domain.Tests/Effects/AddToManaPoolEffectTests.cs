using AwesomeAssertions;
using Clairvoyance.Domain.Effects;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests.Effects;

[TestFixture]
public class AddToManaPoolEffectTests
{
    [Test]
    public void CanGetAvailableMana()
    {
        // Arrange
        var effect = new AddToManaPoolEffect("{C}");

        // Act
        var actualAvailableMana = effect.AvailableMana;
        actualAvailableMana.ToString().Should().Be("AT:0-AC:0-C:1-W:0-U:0-B:0-R:0-G:0");
    }
}
