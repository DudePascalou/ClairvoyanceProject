using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities;

public abstract class StaticAbilityBase : AbilityBase, IStaticAbility
{
    public StaticAbilityBase()
    { }
    public StaticAbilityBase(ICondition condition) : base(condition)
    { }
}