using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Costs;
using Clairvoyance.Domain.Effects;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests.Abilities
{
    [TestFixture]
    public class IAbilityTests
    {
        [Test]
        public void CanTellIsAManaAbility()
        {
            // Arrange - Act - Assert
            Assert.IsFalse(new DrawCardActivatedAbility(new TapCost(), new DrawCardEffect(1)).IsAManaAbility);
            Assert.IsFalse(new FlyingStaticAbility().IsAManaAbility);
            Assert.IsFalse(new HasteStaticAbility().IsAManaAbility);
            Assert.IsTrue(new ManaActivatedAbility(new IsOnBattlefieldCondition(), new TapCost(), new AddToManaPoolEffect("{C}")).IsAManaAbility);
            Assert.IsFalse(new SummoningSicknessStaticAbility().IsAManaAbility);
        }
    }
}
