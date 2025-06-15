﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class AbonnementTests
    {
        private const string id = "00015";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const float montant = 25.32f;
        private static readonly DateTime dateFinAbonnement = DateTime.Now.AddMonths(2);
        private const string idRevue = "0003";
        private static readonly Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);

        [TestMethod()]
        public void AbonnementTest()
        {
            Assert.AreEqual(id, abonnement.Id, "devrait marcher");
            Assert.AreEqual(dateCommande, abonnement.DateCommande, "devrait marcher");
            Assert.AreEqual(montant, abonnement.Montant, "devrait marcher");
            Assert.AreEqual(dateFinAbonnement, abonnement.DateFinAbonnement, "devrait marcher");
            Assert.AreEqual(idRevue, abonnement.IdRevue, "devrait marcher");
        }
    }
}
