using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Costs;
using Clairvoyance.Domain.Effects;
using System;

namespace Clairvoyance.Domain.Abilities
{
    public abstract class ActivatedAbilityBase : AbilityBase, IActivatedAbility
    {
        //public abstract bool IsAManaAbility { get; }
        public override bool IsAManaAbility { get { return false; } }

        public new bool IsAvailable { get { return base.IsAvailable && Cost.CanPay(); } }
        public ICost Cost { get; set; }
        public IEffect Effect { get; set; }

        public ActivatedAbilityBase(ICondition condition, ICost cost, IEffect effect)
        {
            Condition = condition;
            Condition.Ability = this;
            Cost = cost;
            Cost.Ability = this;
            Effect = effect;
            Effect.Ability = this;
        }

        public TCost GetCost<TCost>() where TCost : class, ICost
        {
            return Cost as TCost;
        }

        public TEffect GetEffect<TEffect>() where TEffect : class, IEffect
        {
            return Effect as TEffect;
        }

        public void Activate()
        {
            if (!Condition.IsTrue()) { return; }
            Cost.Pay();
            if (IsAManaAbility)
            {
                // 405.6c Mana abilities resolve immediately.
                Effect.Resolves();
            }
            else
            {
                // TODO : Put on the stack
                throw new NotImplementedException();
            }
        }
    }
}