using AwesomeAssertions;
using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Costs;
using Clairvoyance.Domain.Effects;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests.Abilities;

[TestFixture]
public class ActivatedAbilityTests
{
    [Test]
    public void CanActivate()
    {
        // Arrange - Act - Assert
        var player = new Player(Deck.Empty, new NoPlayerAI());
        player.ManaPool.ToLongString().Should().Be("C:0-W:0-U:0-B:0-R:0-G:0");

        var activatedAbility = new ManaActivatedAbility(new NoCondition(), new TapCost(), new AddToManaPoolEffect("{C}"));
        activatedAbility.Card = Card.Fake;
        activatedAbility.Owner = player;
        activatedAbility.Activate();

        player.ManaPool.ToLongString().Should().Be("C:1-W:0-U:0-B:0-R:0-G:0");
    }
}
