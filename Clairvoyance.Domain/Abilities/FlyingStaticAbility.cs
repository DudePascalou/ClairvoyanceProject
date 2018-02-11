using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities
{
    public class FlyingStaticAbility : StaticAbilityBase, IStaticAbility
    {
        public override bool IsAManaAbility { get { return false; } }

        public FlyingStaticAbility()
        { }
        public FlyingStaticAbility(ICondition condition) : base(condition)
        { }

        public override IAbility Clone()
        {
            return new FlyingStaticAbility(Condition.Clone());
        }
    }
}