using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class GenreTests
    {
        [TestMethod]
        public void GenreTest()
        {
            string id = "00001";
            string libelle = "Science-Fiction";

            Genre genre = new Genre(id, libelle);

            Assert.AreEqual(id, genre.Id, "devrait marcher");
            Assert.AreEqual(libelle, genre.Libelle, "devrait marcher");
            Assert.AreEqual(libelle, genre.ToString(), "devrait marcher");
        }
    }
}
