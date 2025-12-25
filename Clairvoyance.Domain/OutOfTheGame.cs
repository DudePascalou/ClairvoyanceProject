namespace Clairvoyance.Domain;

public class OutOfTheGame : CardSetBase, ICardSet
{
    public OutOfTheGame() : base()
    { }

    public OutOfTheGame(IList<Card> cards) : base(cards)
    { }
}