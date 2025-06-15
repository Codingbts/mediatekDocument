using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class CommandeDocumentTests
    {
        [TestMethod]
        public void CommandeDocumentTest()
        {
            string id = "00025";
            DateTime dateCommande = new DateTime(2025, 6, 15);
            float montant = 150.75f;
            int nbExemplaire = 3;
            string idLivreDvd = "00015";
            int idSuivi = 2;
            string etapeSuivi = "En traitement";

            CommandeDocument commandeDoc = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etapeSuivi);

            Assert.AreEqual(id, commandeDoc.Id, "devrait marcher");
            Assert.AreEqual(dateCommande, commandeDoc.DateCommande, "devrait marcher");
            Assert.AreEqual(montant, commandeDoc.Montant, "devrait marcher");
            Assert.AreEqual(nbExemplaire, commandeDoc.NbExemplaire, "devrait marcher");
            Assert.AreEqual(idLivreDvd, commandeDoc.IdLivreDvd, "devrait marcher");
            Assert.AreEqual(idSuivi, commandeDoc.IdSuivi, "devrait marcher");
            Assert.AreEqual(etapeSuivi, commandeDoc.EtapeSuivi, "devrait marcher");
        }
    }
}
