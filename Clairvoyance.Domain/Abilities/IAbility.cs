using Clairvoyance.Domain.Conditions;

namespace Clairvoyance.Domain.Abilities;

public interface IAbility
{
    Player Owner { get; set; }
    Card Card { get; set; }
    ICondition Condition { get; set; }
    bool IsLegal { get; set; }
    /// <summary>
    /// Tells whether the current ability is a mana ability (605).
    /// </summary>
    bool IsAManaAbility { get; }
    /// <summary>
    /// Tells whether the current ability is available.
    /// </summary>
    bool IsAvailable { get; }

    TCondition GetCondition<TCondition>() where TCondition : class, ICondition;
    IAbility Clone();
}