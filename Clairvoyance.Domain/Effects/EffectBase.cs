using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Effects
{
    public abstract class EffectBase : IEffect
    {
        public IAbility Ability { get; set; }
        public abstract void Resolves();
        public abstract IEffect Clone();
    }
}