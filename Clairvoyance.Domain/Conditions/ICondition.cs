using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Conditions
{
    public interface ICondition
    {
        IAbility Ability { get; set; }
        bool IsTrue();
        ICondition Clone();
    }
}