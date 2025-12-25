namespace Clairvoyance.Domain.Algorithms;

public interface IPlayerAI
{
    Player Player { get; set; }
    /// <summary>
    /// Choose a <see cref="Card"/> to play.
    /// </summary>
    /// <returns>The <see cref="Card"/> to play. Null, otherwise.</returns>
    Card ChooseCard();
}
