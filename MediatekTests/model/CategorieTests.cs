using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class CategorieTests
    {
        private const string id = "00002";
        private const string libelle = "Mangas";
        private static readonly Categorie categorie = new Categorie(id, libelle);

        [TestMethod()]
        public void CategorieTest()
        {
            Assert.AreEqual(id, categorie.Id, "devrait marcher");
            Assert.AreEqual(libelle, categorie.Libelle, "devrait marcher");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(libelle, categorie.ToString(), "devrait marcher");
        }
    }
}
