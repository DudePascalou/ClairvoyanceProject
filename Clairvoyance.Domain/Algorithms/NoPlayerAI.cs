namespace Clairvoyance.Domain.Algorithms
{
    public class NoPlayerAI : IPlayerAI
    {
        public Player Player { get; set; }

        public Card ChooseCard() { return null; }
    }
}