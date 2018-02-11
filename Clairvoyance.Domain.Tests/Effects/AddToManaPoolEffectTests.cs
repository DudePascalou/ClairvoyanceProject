using Clairvoyance.Domain.Effects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clairvoyance.Domain.Tests.Effects
{
    [TestFixture]
    public class AddToManaPoolEffectTests
    {
        [Test]
        public void CanGetAvailableMana()
        {
            // Arrange
            var effect = new AddToManaPoolEffect("{C}");

            // Act
            var actualAvailableMana = effect.AvailableMana;
            Assert.AreEqual("AT:0-AC:0-C:1-W:0-U:0-B:0-R:0-G:0", actualAvailableMana.ToString());
        }
    }
}
