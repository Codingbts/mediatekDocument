using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public void DocumentTest()
        {
            // Arrange
            string id = "00001";
            string titre = "Titre Test";
            string image = "image.jpg";
            string idGenre = "genre1";
            string genre = "Science-Fiction";
            string idPublic = "public1";
            string lePublic = "Adulte";
            string idRayon = "rayon1";
            string rayon = "Rayon A";

            // Act
            Document doc = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Assert
            Assert.AreEqual(id, doc.Id, "devrait marcher");
            Assert.AreEqual(titre, doc.Titre, "devrait marcher");
            Assert.AreEqual(image, doc.Image, "devrait marcher");
            Assert.AreEqual(idGenre, doc.IdGenre, "devrait marcher");
            Assert.AreEqual(genre, doc.Genre, "devrait marcher");
            Assert.AreEqual(idPublic, doc.IdPublic, "devrait marcher");
            Assert.AreEqual(lePublic, doc.Public, "devrait marcher");
            Assert.AreEqual(idRayon, doc.IdRayon, "devrait marcher");
            Assert.AreEqual(rayon, doc.Rayon, "devrait marcher");
        }
    }
}
