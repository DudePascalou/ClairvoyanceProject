using Clairvoyance.Services;
using NUnit.Framework;

namespace Clairvoyance.Domain.Tests;

[TestFixture]
public class GameTests
{
    [Test]
    [Ignore("To be implemented...")]
    public void CanStartNewGame()
    {
        var deck = new MtgCardService().ParseDeckListJson("TODO", Format.Legacy, "TODO");
        var player = new Player(deck, null);
        var game = new Game(player);
        game.Prepare();
        game.Start();
    }
}
