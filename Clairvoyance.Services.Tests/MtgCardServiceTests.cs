using AwesomeAssertions;
using Clairvoyance.Domain;
using NUnit.Framework;

namespace Clairvoyance.Services.Tests;

[TestFixture]
public class MtgCardServiceTests
{
    private static readonly MtgCardService _Service = new MtgCardService();
    private static MtgCardService MtgCardService
    {
        get { return _Service; }
    }

    [Test]
    [Ignore("To be implemented...")] // TODO : Manage aftermath cards...
    public void CanFindByName_WithAftermath()
    {
        var springToMind = MtgCardService.FindByName("Spring // Mind");

    }

    [Test]
    public void CanParseDeckList()
    {
        // Act
        var deck = new MtgCardService().ParseDeckListJson("SimpleSample", Format.Legacy, SampleDeckListsJson.SimpleSample);

        // Assert
        deck.Cards.Should().HaveCount(28);
    }

    [Test]
    [Ignore("To be implemented...")] // TODO : Manage aftermath cards...
    public void CanParseDeckList_WithAftermath()
    {
        // Act
        var deck = new MtgCardService().ParseDeckListJson("SampleWithAftermathLayout", Format.Legacy, SampleDeckListsJson.SampleWithAftermathLayout);

        // Assert
        deck.Cards.Should().ContainSingle();
        deck.SideboardCards.Should().BeEmpty();
    }

}
