using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities;

public abstract class AbilityBase : IAbility
{
    public Player Owner { get; set; }
    public Card Card { get; set; }
    public ICondition Condition { get; set; }
    public bool IsLegal { get; set; }
    public abstract bool IsAManaAbility { get; }
    public bool IsAvailable { get { return Condition.IsTrue(); } }

    public AbilityBase()
    { }
    public AbilityBase(ICondition condition)
    {
        Condition = condition;
    }

    public TCondition GetCondition<TCondition>() where TCondition : class, ICondition
    {
        return Condition as TCondition;
    }
    public abstract IAbility Clone();
}