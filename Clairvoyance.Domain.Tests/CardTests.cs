using AwesomeAssertions;
using Clairvoyance.Domain.Abilities;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests;

[TestFixture]
public class CardTests
{
    [Test]
    public void CanTellIsAnArtifact()
    {
        // Arrange
        var artifact = CardSamples.Artifact;

        // Act - Assert
        artifact.IsAnArtifact.Should().BeTrue();
        artifact.IsACreature.Should().BeFalse();
        artifact.IsAnEnchantment.Should().BeFalse();
        artifact.IsAnInstant.Should().BeFalse();
        artifact.IsALand.Should().BeFalse();
        artifact.IsAPlaneswalker.Should().BeFalse();
        artifact.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsACreature()
    {
        // Arrange
        var creature = CardSamples.Creature;

        // Act - Assert
        creature.IsAnArtifact.Should().BeFalse();
        creature.IsACreature.Should().BeTrue();
        creature.IsAnEnchantment.Should().BeFalse();
        creature.IsAnInstant.Should().BeFalse();
        creature.IsALand.Should().BeFalse();
        creature.IsAPlaneswalker.Should().BeFalse();
        creature.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsAnEnchantment()
    {
        // Arrange
        var enchantment = CardSamples.Enchantment;

        // Act - Assert
        enchantment.IsAnArtifact.Should().BeFalse();
        enchantment.IsACreature.Should().BeFalse();
        enchantment.IsAnEnchantment.Should().BeTrue();
        enchantment.IsAnInstant.Should().BeFalse();
        enchantment.IsALand.Should().BeFalse();
        enchantment.IsAPlaneswalker.Should().BeFalse();
        enchantment.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsAnInstant()
    {
        // Arrange
        var instant = CardSamples.Instant;

        // Act - Assert
        instant.IsAnArtifact.Should().BeFalse();
        instant.IsACreature.Should().BeFalse();
        instant.IsAnEnchantment.Should().BeFalse();
        instant.IsAnInstant.Should().BeTrue();
        instant.IsALand.Should().BeFalse();
        instant.IsAPlaneswalker.Should().BeFalse();
        instant.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsALand()
    {
        // Arrange
        var land = CardSamples.Land;

        // Act - Assert
        land.IsAnArtifact.Should().BeFalse();
        land.IsACreature.Should().BeFalse();
        land.IsAnEnchantment.Should().BeFalse();
        land.IsAnInstant.Should().BeFalse();
        land.IsALand.Should().BeTrue();
        land.IsAPlaneswalker.Should().BeFalse();
        land.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsAPlaneswalker()
    {
        // Arrange
        var planeswalker = CardSamples.Planeswalker;

        // Act - Assert
        planeswalker.IsAnArtifact.Should().BeFalse();
        planeswalker.IsACreature.Should().BeFalse();
        planeswalker.IsAnEnchantment.Should().BeFalse();
        planeswalker.IsAnInstant.Should().BeFalse();
        planeswalker.IsALand.Should().BeFalse();
        planeswalker.IsAPlaneswalker.Should().BeTrue();
        planeswalker.IsASorcery.Should().BeFalse();
    }

    [Test]
    public void CanTellIsASorcery()
    {
        // Arrange
        var sorcery = CardSamples.Sorcery;

        // Act - Assert
        sorcery.IsAnArtifact.Should().BeFalse();
        sorcery.IsACreature.Should().BeFalse();
        sorcery.IsAnEnchantment.Should().BeFalse();
        sorcery.IsAnInstant.Should().BeFalse();
        sorcery.IsALand.Should().BeFalse();
        sorcery.IsAPlaneswalker.Should().BeFalse();
        sorcery.IsASorcery.Should().BeTrue();
    }

    [Test]
    public void CanTellHasAbility()
    {
        // Arrange
        var card = new Card();

        // Assert
        card.HasAbility<FlyingStaticAbility>().Should().BeFalse();
        card.HasAbility<SummoningSicknessStaticAbility>().Should().BeFalse();

        card.Abilities.Add(new SummoningSicknessStaticAbility());

        // Act - Assert
        card.HasAbility<FlyingStaticAbility>().Should().BeFalse();
        card.HasAbility<SummoningSicknessStaticAbility>().Should().BeTrue();
    }

    [Test]
    public void CanGetAbility()
    {
        // Arrange
        var card = new Card();

        // Assert
        card.GetAbility<FlyingStaticAbility>().Should().BeNull();
        card.GetAbility<SummoningSicknessStaticAbility>().Should().BeNull();

        card.Abilities.Add(new SummoningSicknessStaticAbility());

        // Act - Assert
        card.GetAbility<FlyingStaticAbility>().Should().BeNull();
        card.GetAbility<SummoningSicknessStaticAbility>().Should().BeAssignableTo<SummoningSicknessStaticAbility>();
    }
}
