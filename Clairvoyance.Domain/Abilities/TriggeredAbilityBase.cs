using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Effects;

namespace Clairvoyance.Domain.Abilities
{
    public abstract class TriggeredAbilityBase : AbilityBase, ITriggeredAbility
    {
        public override bool IsAManaAbility { get { return false; } }
        public IEffect Effect { get; set; }
        
        public TriggeredAbilityBase()
        { }
        public TriggeredAbilityBase(ICondition condition, IEffect effect) : base(condition)
        {
            Effect = effect;
        }

        public TEffect GetEffect<TEffect>() where TEffect : class, IEffect
        {
            return Effect as TEffect;
        }
    }
}