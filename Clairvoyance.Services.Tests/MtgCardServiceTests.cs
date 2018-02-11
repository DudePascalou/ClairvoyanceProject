using Clairvoyance.Domain;
using Clairvoyance.Services;
using Clairvoyance.Services.Tests.Resources;
using NUnit.Framework;

namespace mtgtools.Tests.Services
{
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
            Assert.AreEqual(28, deck.Cards.Count);
        }

        [Test]
        [Ignore("To be implemented...")] // TODO : Manage aftermath cards...
        public void CanParseDeckList_WithAftermath()
        {
            // Act
            var deck = new MtgCardService().ParseDeckListJson("SampleWithAftermathLayout", Format.Legacy, SampleDeckListsJson.SampleWithAftermathLayout);

            // Assert
            Assert.AreEqual(1, deck.Cards.Count);
            Assert.AreEqual(0, deck.SideboardCards.Count);
        }

    }
}
