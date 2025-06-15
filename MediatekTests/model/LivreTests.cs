using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class LivreTests
    {
        [TestMethod]
        public void LivreTest()
        {
            string id = "00001";
            string titre = "1984";
            string image = "image.jpg";
            string isbn = "978-0451524935";
            string auteur = "George Orwell";
            string collection = "Classiques";
            string idGenre = "00001";
            string genre = "Dystopie";
            string idPublic = "00001";
            string lePublic = "Adultes";
            string idRayon = "00002";
            string rayon = "Roman";

            Livre livre = new Livre(id, titre, image, isbn, auteur, collection,
                idGenre, genre, idPublic, lePublic, idRayon, rayon);

            Assert.AreEqual(id, livre.Id, "devrait marcher");
            Assert.AreEqual(titre, livre.Titre, "devrait marcher");
            Assert.AreEqual(image, livre.Image, "devrait marcher");
            Assert.AreEqual(isbn, livre.Isbn, "devrait marcher");
            Assert.AreEqual(auteur, livre.Auteur, "devrait marcher");
            Assert.AreEqual(collection, livre.Collection, "devrait marcher");
            Assert.AreEqual(idGenre, livre.IdGenre, "devrait marcher");
            Assert.AreEqual(genre, livre.Genre, "devrait marcher");
            Assert.AreEqual(idPublic, livre.IdPublic, "devrait marcher");
            Assert.AreEqual(lePublic, livre.Public, "devrait marcher");
            Assert.AreEqual(idRayon, livre.IdRayon, "devrait marcher");
            Assert.AreEqual(rayon, livre.Rayon, "devrait marcher");
        }
    }
}
