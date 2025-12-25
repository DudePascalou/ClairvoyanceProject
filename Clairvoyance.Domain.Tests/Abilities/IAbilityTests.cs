using AwesomeAssertions;
using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Costs;
using Clairvoyance.Domain.Effects;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests.Abilities;

[TestFixture]
public class IAbilityTests
{
    [Test]
    public void CanTellIsAManaAbility()
    {
        // Arrange - Act - Assert
        new DrawCardActivatedAbility(new TapCost(), new DrawCardEffect(1)).IsAManaAbility.Should().BeFalse();
        new FlyingStaticAbility().IsAManaAbility.Should().BeFalse();
        new HasteStaticAbility().IsAManaAbility.Should().BeFalse();
        new ManaActivatedAbility(new IsOnBattlefieldCondition(), new TapCost(), new AddToManaPoolEffect("{C}")).IsAManaAbility.Should().BeTrue();
        new SummoningSicknessStaticAbility().IsAManaAbility.Should().BeFalse();
    }
}
