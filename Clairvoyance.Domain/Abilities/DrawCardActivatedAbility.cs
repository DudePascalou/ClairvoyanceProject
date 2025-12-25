using Clairvoyance.Domain.Conditions;
using Clairvoyance.Domain.Costs;
using Clairvoyance.Domain.Effects;

namespace Clairvoyance.Domain.Abilities;

public class DrawCardActivatedAbility : ActivatedAbilityBase, IActivatedAbility
{
    public override bool IsAManaAbility { get { return false; } }
    public DrawCardActivatedAbility(ICondition condition, ICost cost, IEffect effect) : base(condition, cost, effect) { }
    public DrawCardActivatedAbility(ICost cost, IEffect effect) : base(new NoCondition(), cost, effect) { }

    public override IAbility Clone()
    {
        return new DrawCardActivatedAbility
        (
            Condition.Clone(),
            Cost.Clone(),
            Effect.Clone()
        );
    }
}