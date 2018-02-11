using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions
{
    public class IsOnBattlefieldCondition : ICondition
    {
        public IAbility Ability { get; set; }
        public ICondition Clone() { return new IsOnBattlefieldCondition(); }
        public bool IsTrue() { return Ability.Owner.Battlefield.Cards.Contains(Ability.Card); }
    }
}