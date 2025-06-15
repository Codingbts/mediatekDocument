using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod]
        public void ServiceTest()
        {
            int id = 1;
            string libelle = "Informatique";

            Service service = new Service(id, libelle);

            Assert.AreEqual(id, service.Id, "devrait marcher");
            Assert.AreEqual(libelle, service.Libelle, "devrait marcher");
            Assert.AreEqual(libelle, service.ToString(), "devrait marcher");
        }
    }
}
