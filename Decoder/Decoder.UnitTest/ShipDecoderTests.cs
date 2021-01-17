using Decoder.Logic;
using NUnit.Framework;

namespace Decoder.UnitTest
{
    public class ShipDecoderTests
    {
        private ShipDecoder decoder;
        [SetUp]
        public void Setup()
        {
            var satellites = new Satellite[] { new Satellite { Name = "kenobi", Possition = (-500, -200) }, new Satellite { Name = "skywalker", Possition = (100, -100) }, new Satellite { Name = "sato", Possition = (500, 100) } };
            decoder = new ShipDecoder(satellites);
        }

        [Test]
        public void Calculate_Distance_To_Vessel()
        {
            var distances = new double[] {100.0, 115.5, 142.7 };
            var retval = decoder.GetLocation(distances);
            Assert.AreEqual((-499.8, 1532.0), retval);
        }

        [Test]
        public void Get_Encoded_Message()
        {
            var messages = new string[][] { new string[] { "este", "","","mensaje", ""}, new string[] { "", "es", "", "", "secreto" }, new string[] { "este", "", "un", "", "" } };
            var retval = decoder.GetMessage(messages);
            Assert.AreEqual("este es un mensaje secreto", retval);
        }

        [Test]
        public void Normalize_And_Get_Encoded_Message()
        {
            var messages = new string[][] { new string[] { "", "este", "", "", "mensaje", "" }, new string[] { "", "es", "", "", "secreto" }, new string[] { "este", "", "un", "", "", "" } };
            var retval = decoder.GetMessage(messages);
            Assert.AreEqual("este es un mensaje secreto", retval);
        }
    }
}