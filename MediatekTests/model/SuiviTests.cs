using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class SuiviTests
    {
        [TestMethod]
        public void SuiviTest()
        {
            int idSuivi = 10;
            string etapeSuivi = "En cours de traitement";

            Suivi suivi = new Suivi(idSuivi, etapeSuivi);

            Assert.AreEqual(idSuivi, suivi.IdSuivi, "devrait marcher");
            Assert.AreEqual(etapeSuivi, suivi.EtapeSuivi, "devrait marcher");
            Assert.AreEqual(etapeSuivi, suivi.ToString(), "devrait marcher");
        }
    }
}
