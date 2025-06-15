using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class RayonTests
    {
        [TestMethod]
        public void RayonTest()
        {
            string id = "00005";
            string libelle = "Roman";

            Rayon rayon = new Rayon(id, libelle);

            Assert.AreEqual(id, rayon.Id, "devrait marcher");
            Assert.AreEqual(libelle, rayon.Libelle, "devrait marcher");
            Assert.AreEqual(libelle, rayon.ToString(), "devrait marcher");
        }
    }
}
