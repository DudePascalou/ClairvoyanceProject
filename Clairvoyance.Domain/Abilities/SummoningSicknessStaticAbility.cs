using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities;

/// <summary>
/// 302.6. A creature’s activated ability with the tap symbol or the untap symbol in its activation cost 
/// can’t be activated unless the creature has been under its controller’s control 
/// continuously since his or her most recent turn began. 
/// A creature can’t attack unless it has been under its controller’s control 
/// continuously since his or her most recent turn began. 
/// This rule is informally called the “summoning sickness” rule.
/// </summary>
public class SummoningSicknessStaticAbility : StaticAbilityBase, IStaticAbility
{
    public override bool IsAManaAbility { get { return false; } }

    public SummoningSicknessStaticAbility() : base(new NoCondition())
    { }

    public override IAbility Clone()
    {
        return new SummoningSicknessStaticAbility();
    }
}