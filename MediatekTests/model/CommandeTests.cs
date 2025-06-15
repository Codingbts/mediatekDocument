using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class CommandeTests
    {
        private const string id = "00001";
        private static readonly DateTime dateCommande = new DateTime(2023, 6, 15);
        private const float montant = 100.5f;
        private static readonly Commande commande = new Commande(id, dateCommande, montant);

        [TestMethod()]
        public void CommandeTest()
        {
            Assert.AreEqual(id, commande.Id, "devrait marcher");
            Assert.AreEqual(dateCommande, commande.DateCommande, "devrait marcher");
            Assert.AreEqual(montant, commande.Montant, "devrait marcher");
        }
    }
}
