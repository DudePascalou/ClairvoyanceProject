using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;
using System.Linq;

namespace Clairvoyance.Domain.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void CanStartGame()
        {
            // Arrange
            var deck = DeckBuilder.Build("CanStartGame", 16);
            var player = new Player(deck, new NoPlayerAI());
            var expectedStartingHand = 7;
            var expectedStartingLibrary = deck.Cards.Count - expectedStartingHand;

            // Act
            player.Draw(expectedStartingHand);

            // Assert
            Assert.AreEqual(expectedStartingHand, player.Hand.Cards.Count);
            Assert.AreEqual(expectedStartingLibrary, player.Library.Cards.Count);
        }

        [Test]
        public void CanTakeMulligan()
        {
            // Arrange
            var deck = DeckBuilder.Build("CanTakeMulligan", 16);
            var firstMulligan = 4;
            var player = new Player(deck, new NoPlayerAI(), 20, firstMulligan);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(4, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count - 4, player.Library.Cards.Count);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(3, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count - 3, player.Library.Cards.Count);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(2, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count - 2, player.Library.Cards.Count);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(1, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count - 1, player.Library.Cards.Count);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(0, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count, player.Library.Cards.Count);

            // Act
            player.TakeMulligan();

            // Assert
            Assert.AreEqual(0, player.Hand.Cards.Count);
            Assert.AreEqual(deck.Cards.Count, player.Library.Cards.Count);
        }

        [Test]
        public void CanGetAvailableMana()
        {
            // Arrange
            var deck = DeckBuilder.Build("CanGetAvailableMana")
                .With(CardJson.BaneOfBalaGed)
                .With(CardJson.ThoughtKnotSeer)
                .With(CardJson.ElvishSpiritGuide)
                .With(CardJson.MagusOfTheLibrary)
                .With(CardJson.Forest)
                .With(CardJson.SpawningBed)
                .With(CardJson.GhostQuarter);
            var player = new Player(deck, new NoPlayerAI());
            player.ShuffleLibrary();
            player.Draw(7);
            var expectedAvailableMana = new AvailableMana();

            // Act
            var actualAvailableMana = player.GetAvailableMana();

            // Assert
            Assert.AreEqual("AT:0-AC:0-C:0-W:0-U:0-B:0-R:0-G:1", actualAvailableMana.ToString());

            // Arrange
            var forest = player.Hand.Get("Forest");
            player.Play(forest);

            // Act
            actualAvailableMana = player.GetAvailableMana();

            // Assert
            Assert.AreEqual("AT:0-AC:0-C:0-W:0-U:0-B:0-R:0-G:2", actualAvailableMana.ToString());

            // Arrange
            var magus = player.Hand.Cards.First(c => c.Name == "Magus of the Library");
            player.Play(magus); // Use Forest & Elvish Spirit Guide

            // Act
            actualAvailableMana = player.GetAvailableMana();

            // Assert
            Assert.AreEqual("AT:0-AC:0-C:0-W:0-U:0-B:0-R:0-G:0", actualAvailableMana.ToString());

            // Arrange
            player.PassTheTurn();
            player.StartTheTurn();

            // Act
            actualAvailableMana = player.GetAvailableMana();

            // Assert
            Assert.AreEqual("AT:0-AC:0-C:1-W:0-U:0-B:0-R:0-G:1", actualAvailableMana.ToString());

            // Arrange
            var ghostQuarter = player.Hand.Cards.First(c => c.Name == "Ghost Quarter");
            player.Play(ghostQuarter);

            // Act
            actualAvailableMana = player.GetAvailableMana();

            // Assert
            Assert.AreEqual("AT:0-AC:0-C:2-W:0-U:0-B:0-R:0-G:1", actualAvailableMana.ToString());
        }

        [Test]
        [Ignore("To be implemented...")]
        public void CanPayManaCost()
        {
            // Arrange
            var deck = DeckBuilder.Build("CanPayManaCost");
            var player = new Player(deck, new NoPlayerAI());

            // Act

            // Assert

        }
    }
}
