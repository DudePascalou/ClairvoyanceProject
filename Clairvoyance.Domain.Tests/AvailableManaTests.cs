using NUnit.Framework;

namespace Clairvoyance.Domain.Tests
{
    [TestFixture]
    public class AvailableManaTests
    {
        [Test]
        public void CanTellWhetherIsEnoughFor()
        {
            // Arrange - Act - Assert
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{G}")));
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{W}")));
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}{G}")));
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}{W}")));
            Assert.IsFalse(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}")));
            Assert.IsFalse(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{G}{G}")));
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{1}")));
            Assert.IsTrue(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{2}")));
            Assert.IsFalse(new AvailableMana("{W}{G}").IsEnoughFor(TypedMana.Parse("{3}")));

            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}")));
            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}")));
            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}{W}")));
            Assert.IsFalse(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{W}{W}{W}{W}")));

            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{R}{G}")));
            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{G}")));
            Assert.IsTrue(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{R}{G}")));
            Assert.IsFalse(new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}").IsEnoughFor(TypedMana.Parse("{B}{B}{R}{R}{G}{G}")));

            // TODO bilands...
        }

        [Test]
        public void CanClone()
        {
            // Arrange
            var expectedAvailableMana = new AvailableMana("{AC}{AT}{C}{W}{U}{B}{R}{G}");

            // Act
            var actualAvailableMana = expectedAvailableMana.Clone();

            // Assert
            Assert.AreEqual(expectedAvailableMana.ToString(), actualAvailableMana.ToString());
        }
    }
}
