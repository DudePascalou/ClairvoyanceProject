using NUnit.Framework;

namespace Clairvoyance.Domain.Tests
{
    [TestFixture]
    public class TypedManaTests
    {
        [Test]
        public void CanParse()
        {
            // Arrange - Act - Assert
            var typedMana = TypedMana.Parse("{0}");
            Assert.AreEqual(0, typedMana.Cmc);
            Assert.AreEqual("X:0-G:0-C:0-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{1}{C}");
            Assert.AreEqual(2, typedMana.Cmc);
            Assert.AreEqual("X:0-G:1-C:1-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{2}{U}");
            Assert.AreEqual(3, typedMana.Cmc);
            Assert.AreEqual("X:0-G:2-C:0-W:0-U:1-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{B}{B}");
            Assert.AreEqual(2, typedMana.Cmc);
            Assert.AreEqual("X:0-G:0-C:0-W:0-U:0-B:2-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{R}{R}{R}");
            Assert.AreEqual(3, typedMana.Cmc);
            Assert.AreEqual("X:0-G:0-C:0-W:0-U:0-B:0-R:3-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{G}{U}{G}{R}");
            Assert.AreEqual(4, typedMana.Cmc);
            Assert.AreEqual("X:0-G:0-C:0-W:0-U:1-B:0-R:1-G:2", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{X}");
            Assert.AreEqual(0, typedMana.Cmc);
            Assert.AreEqual("X:1-G:0-C:0-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{X}{X}");
            Assert.AreEqual(0, typedMana.Cmc);
            Assert.AreEqual("X:2-G:0-C:0-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{X}{X}{X}");
            Assert.AreEqual(0, typedMana.Cmc);
            Assert.AreEqual("X:3-G:0-C:0-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{X}{X}{5}");
            Assert.AreEqual(5, typedMana.Cmc);
            Assert.AreEqual("X:2-G:5-C:0-W:0-U:0-B:0-R:0-G:0", typedMana.ToLongString());
            typedMana = TypedMana.Parse("{X}{2}{C}{C}{C}{W}{W}{W}{W}{U}{U}{U}{U}{U}{B}{B}{B}{B}{B}{B}{R}{R}{R}{R}{R}{R}{R}{G}{G}{G}{G}{G}{G}{G}{G}");
            Assert.AreEqual(35, typedMana.Cmc);
            Assert.AreEqual("X:1-G:2-C:3-W:4-U:5-B:6-R:7-G:8", typedMana.ToLongString());
        }

        [Test]
        public void CanAdd()
        {
            // Arrange - Act - Assert
            var typedMana1 = TypedMana.Parse("{1}");
            var typedMana2 = TypedMana.Parse("{1}");
            Assert.AreEqual("X:0-G:2-C:0-W:0-U:0-B:0-R:0-G:0", (typedMana1 + typedMana2).ToLongString());
            typedMana1 = TypedMana.Parse("{W}{U}");
            typedMana2 = TypedMana.Parse("{2}{W}{B}{B}");
            Assert.AreEqual("X:0-G:2-C:0-W:2-U:1-B:2-R:0-G:0", (typedMana1 + typedMana2).ToLongString());
            typedMana1 = TypedMana.Parse("{C}{C}");
            typedMana2 = TypedMana.Parse("{C}{R}{R}{G}");
            Assert.AreEqual("X:0-G:0-C:3-W:0-U:0-B:0-R:2-G:1", (typedMana1 + typedMana2).ToLongString());
        }
    }
}
