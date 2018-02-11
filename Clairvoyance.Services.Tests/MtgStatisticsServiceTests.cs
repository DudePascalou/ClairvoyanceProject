using Clairvoyance.Domain;
using Clairvoyance.Services;
using Clairvoyance.Services.Tests.Models;
using Clairvoyance.Tests.Resources;
using NUnit.Framework;

namespace mtgtools.Tests.Services
{
    [TestFixture]
    public class MtgStatisticsServiceTests
    {
        [Test]
        public void CanComputePlayabilityStatistics()
        {
            var deck = DeckBuilder.Build("CanComputePlayabilityStatistics")
                .With(1, CardJson.NissaStewardOfElements)
                .With(1, CardJson.BaneOfBalaGed)
                .With(1, CardJson.DeceiverOfForm)
                .With(1, CardJson.ConduitOfRuin)
                .With(3, CardJson.Endbringer)
                .With(1, CardJson.DeepfathomSkulker)
                .With(1, CardJson.ThoughtKnotSeer)
                .With(4, CardJson.ElvishSpiritGuide)
                .With(3, CardJson.VoidGrafter)
                .With(4, CardJson.MagusOfTheLibrary)
                .With(1, CardJson.DesolationTwin)
                .With(4, CardJson.LlanowarElves)
                .With(4, CardJson.ProphetOfDistortion)
                .With(1, CardJson.HedronArchive)
                .With(4, CardJson.PulseOfMurasa)
                .With(3, CardJson.SpatialContortion)
                .With(3, CardJson.WorldlyTutor)
                .With(2, CardJson.LotusPetal)
                .With(5, CardJson.Island)
                .With(9, CardJson.Forest)
                .With(3, CardJson.GhostQuarter)
                .With(3, CardJson.MishrasFactory)
                .With(1, CardJson.SpawningBed)
                .With(1, CardJson.UnclaimedTerritory);
            var statsSvc = new MtgStatisticsService();
            var playerAI = new DesolationPlayerAI();
            var player = new Player(deck, playerAI);

            statsSvc.ComputePlayabilityStatistics(deck, player, 8);
        }
    }
}
