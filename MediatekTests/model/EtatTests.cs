using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class EtatTests
    {
        private const string id = "00020";
        private const string libelle = "Neuf";

        private static readonly Etat etat = new Etat(id, libelle);

        [TestMethod()]
        public void EtatTest()
        {
            Assert.AreEqual(id, etat.Id, "devrait marcher");
            Assert.AreEqual(libelle, etat.Libelle, "devrait marcher");
        }
    }
}
