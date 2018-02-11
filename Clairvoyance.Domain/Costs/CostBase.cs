using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Costs
{
    public abstract class CostBase : ICost
    {
        public IAbility Ability { get; set; }
        public abstract bool CanPay();
        public abstract void Pay();
        public abstract ICost Clone();
    }
}