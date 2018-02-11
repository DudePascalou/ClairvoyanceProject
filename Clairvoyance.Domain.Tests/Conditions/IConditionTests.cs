using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;
using System.Linq;

namespace Clairvoyance.Domain.Tests.Conditions
{
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
            Assert.IsTrue(esg.GetAbility<ManaActivatedAbility>().Condition.IsTrue());

            // Act - Assert
            player.Hand.Pop(esg);
            player.Battlefield.Enter(esg);
            Assert.IsFalse(esg.GetAbility<ManaActivatedAbility>().Condition.IsTrue());
        }
    }
}
