using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class UtilisateurTests
    {
        [TestMethod]
        public void UtilisateurTest()
        {
            int id = 42;
            string login = "admin";
            string password = "secret";
            int idService = 2;
            string service = "Support";

            Utilisateur utilisateur = new Utilisateur(id, login, password, idService, service);

            Assert.AreEqual(id, utilisateur.Id, "devrait marcher");
            Assert.AreEqual(login, utilisateur.Login, "devrait marcher");
            Assert.AreEqual(password, utilisateur.Password, "devrait marcher");
            Assert.AreEqual(idService, utilisateur.IdService, "devrait marcher");
            Assert.AreEqual(service, utilisateur.Service, "devrait marcher");
        }
    }
}
