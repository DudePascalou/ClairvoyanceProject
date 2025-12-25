using AwesomeAssertions;
using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests.Conditions;

[TestFixture]
public class IConditionTests
{
    [Test]
    public void CanTellIsTrue()
    {
        // Arrange
        var deck = DeckBuilder.Build("CanTellIsTrue")
            .With(CardJson.ElvishSpiritGuide);
        var player = new Player(deck, new NoPlayerAI());
        var esg = player.Deck.Cards.First();

        // Act - Assert
        player.Draw(1);
        esg.GetAbility<ManaActivatedAbility>().Condition.IsTrue().Should().BeTrue();

        // Act - Assert
        player.Hand.Pop(esg);
        player.Battlefield.Enter(esg);
        esg.GetAbility<ManaActivatedAbility>().Condition.IsTrue().Should().BeFalse();
    }
}
