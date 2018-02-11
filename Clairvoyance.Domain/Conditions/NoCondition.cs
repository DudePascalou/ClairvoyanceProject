using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions
{
    public class NoCondition : ICondition
    {
        public IAbility Ability { get; set; }
        public ICondition Clone() { return new NoCondition(); }
        public bool IsTrue() { return true; }
    }
}