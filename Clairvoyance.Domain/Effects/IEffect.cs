using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Effects
{
    public interface IEffect : IStackable
    {
        IAbility Ability { get; set; }
        IEffect Clone();
    }
}