using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions;

public class CanTapCondition : ICondition
{
    public IAbility Ability { get; set; }
    public ICondition Clone() { return new CanTapCondition(); }
    public bool IsTrue() { return Ability.Card.CanTap(); }
}
