using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class PublicTests
    {
        [TestMethod]
        public void PublicTest()
        {
            string id = "00001";
            string libelle = "Adultes";

            Public pub = new Public(id, libelle);

            Assert.AreEqual(id, pub.Id, "devrait marcher");
            Assert.AreEqual(libelle, pub.Libelle, "devrait marcher");
            Assert.AreEqual(libelle, pub.ToString(), "devrait marcher");
        }
    }
}
