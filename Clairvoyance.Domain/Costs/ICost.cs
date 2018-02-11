using Clairvoyance.Domain.Abilities;

namespace Clairvoyance.Domain.Costs
{
    public interface ICost
    {
        IAbility Ability { get; set; }
        bool CanPay();
        void Pay();
        ICost Clone();
    }
}