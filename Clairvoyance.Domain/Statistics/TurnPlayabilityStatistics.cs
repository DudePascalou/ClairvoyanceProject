namespace Clairvoyance.Domain.Statistics;

/// <summary>
/// Gather playability statistics at the end of a turn.
/// Playability is based on what cards can be played on first turns of a game
/// (ignoring what opponent is doing).
/// </summary>
public class TurnPlayabilityStatistics
{
    public IList<Card> ManaAvailable { get; set; }
    /// <summary>
    /// Lands on the battlefield.
    /// </summary>
    public IList<Card> Lands { get; set; }
    /// <summary>
    /// Creatures on the battlefield.
    /// </summary>
    public IList<Card> Creatures { get; set; }
    public int TotalPower { get { return Creatures.Sum(c => int.Parse(c.Power)); } }
    public int TotalToughness { get { return Creatures.Sum(c => int.Parse(c.Toughness)); } }
}