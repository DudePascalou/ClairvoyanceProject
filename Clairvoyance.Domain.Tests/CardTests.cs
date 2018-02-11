using Clairvoyance.Domain.Abilities;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests
{
    [TestFixture]
    public class CardTests
    {
        [Test]
        public void CanTellIsAnArtifact()
        {
            // Arrange
            var artifact = CardSamples.Artifact;

            // Act - Assert
            Assert.IsTrue(artifact.IsAnArtifact);
            Assert.IsFalse(artifact.IsACreature);
            Assert.IsFalse(artifact.IsAnEnchantment);
            Assert.IsFalse(artifact.IsAnInstant);
            Assert.IsFalse(artifact.IsALand);
            Assert.IsFalse(artifact.IsAPlaneswalker);
            Assert.IsFalse(artifact.IsASorcery);
        }

        [Test]
        public void CanTellIsACreature()
        {
            // Arrange
            var creature = CardSamples.Creature;

            // Act - Assert
            Assert.IsFalse(creature.IsAnArtifact);
            Assert.IsTrue(creature.IsACreature);
            Assert.IsFalse(creature.IsAnEnchantment);
            Assert.IsFalse(creature.IsAnInstant);
            Assert.IsFalse(creature.IsALand);
            Assert.IsFalse(creature.IsAPlaneswalker);
            Assert.IsFalse(creature.IsASorcery);
        }

        [Test]
        public void CanTellIsAnEnchantment()
        {
            // Arrange
            var enchantment = CardSamples.Enchantment;

            // Act - Assert
            Assert.IsFalse(enchantment.IsAnArtifact);
            Assert.IsFalse(enchantment.IsACreature);
            Assert.IsTrue(enchantment.IsAnEnchantment);
            Assert.IsFalse(enchantment.IsAnInstant);
            Assert.IsFalse(enchantment.IsALand);
            Assert.IsFalse(enchantment.IsAPlaneswalker);
            Assert.IsFalse(enchantment.IsASorcery);
        }

        [Test]
        public void CanTellIsAnInstant()
        {
            // Arrange
            var instant = CardSamples.Instant;

            // Act - Assert
            Assert.IsFalse(instant.IsAnArtifact);
            Assert.IsFalse(instant.IsACreature);
            Assert.IsFalse(instant.IsAnEnchantment);
            Assert.IsTrue(instant.IsAnInstant);
            Assert.IsFalse(instant.IsALand);
            Assert.IsFalse(instant.IsAPlaneswalker);
            Assert.IsFalse(instant.IsASorcery);
        }

        [Test]
        public void CanTellIsALand()
        {
            // Arrange
            var land = CardSamples.Land;

            // Act - Assert
            Assert.IsFalse(land.IsAnArtifact);
            Assert.IsFalse(land.IsACreature);
            Assert.IsFalse(land.IsAnEnchantment);
            Assert.IsFalse(land.IsAnInstant);
            Assert.IsTrue(land.IsALand);
            Assert.IsFalse(land.IsAPlaneswalker);
            Assert.IsFalse(land.IsASorcery);
        }

        [Test]
        public void CanTellIsAPlaneswalker()
        {
            // Arrange
            var planeswalker = CardSamples.Planeswalker;

            // Act - Assert
            Assert.IsFalse(planeswalker.IsAnArtifact);
            Assert.IsFalse(planeswalker.IsACreature);
            Assert.IsFalse(planeswalker.IsAnEnchantment);
            Assert.IsFalse(planeswalker.IsAnInstant);
            Assert.IsFalse(planeswalker.IsALand);
            Assert.IsTrue(planeswalker.IsAPlaneswalker);
            Assert.IsFalse(planeswalker.IsASorcery);
        }

        [Test]
        public void CanTellIsASorcery()
        {
            // Arrange
            var sorcery = CardSamples.Sorcery;

            // Act - Assert
            Assert.IsFalse(sorcery.IsAnArtifact);
            Assert.IsFalse(sorcery.IsACreature);
            Assert.IsFalse(sorcery.IsAnEnchantment);
            Assert.IsFalse(sorcery.IsAnInstant);
            Assert.IsFalse(sorcery.IsALand);
            Assert.IsFalse(sorcery.IsAPlaneswalker);
            Assert.IsTrue(sorcery.IsASorcery);
        }

        [Test]
        public void CanTellHasAbility()
        {
            // Arrange
            var card = new Card();

            // Assert
            Assert.IsFalse(card.HasAbility<FlyingStaticAbility>());
            Assert.IsFalse(card.HasAbility<SummoningSicknessStaticAbility>());

            card.Abilities.Add(new SummoningSicknessStaticAbility());

            // Act - Assert
            Assert.IsFalse(card.HasAbility<FlyingStaticAbility>());
            Assert.IsTrue(card.HasAbility<SummoningSicknessStaticAbility>());
        }

        [Test]
        public void CanGetAbility()
        {
            // Arrange
            var card = new Card();

            // Assert
            Assert.AreEqual(null, card.GetAbility<FlyingStaticAbility>());
            Assert.AreEqual(null, card.GetAbility<SummoningSicknessStaticAbility>());

            card.Abilities.Add(new SummoningSicknessStaticAbility());

            // Act - Assert
            Assert.AreEqual(null, card.GetAbility<FlyingStaticAbility>());
            Assert.That(card.GetAbility<SummoningSicknessStaticAbility>(), Is.InstanceOf<SummoningSicknessStaticAbility>());
        }

    }
}
