using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities
{
    public class HasteStaticAbility : StaticAbilityBase, IStaticAbility
    {
        public override bool IsAManaAbility { get { return false; } }

        public HasteStaticAbility()
        { }
        public HasteStaticAbility(ICondition condition) : base(condition)
        { }

        public override IAbility Clone()
        {
            return new HasteStaticAbility(Condition.Clone());
        }
    }
}