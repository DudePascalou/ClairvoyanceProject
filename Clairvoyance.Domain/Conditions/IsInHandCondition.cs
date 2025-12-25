using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions;

public class IsInHandCondition : ICondition
{
    public IAbility Ability { get; set; }
    public ICondition Clone() { return new IsInHandCondition(); }
    public bool IsTrue() { return Ability.Owner.Hand.Cards.Contains(Ability.Card); }
}