using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class RevueTests
    {
        [TestMethod]
        public void RevueTest()
        {
            string id = "00004";
            string titre = "Science & Vie";
            string image = "image.png";
            string idGenre = "00045";
            string genre = "Science";
            string idPublic = "00012";
            string lePublic = "Adultes";
            string idRayon = "00025";
            string rayon = "Magazines";
            string periodicite = "Mensuelle";
            int delaiMiseADispo = 10;

            Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

            Assert.AreEqual(id, revue.Id, "devrait marcher");
            Assert.AreEqual(titre, revue.Titre, "devrait marcher");
            Assert.AreEqual(image, revue.Image, "devrait marcher");
            Assert.AreEqual(idGenre, revue.IdGenre, "devrait marcher");
            Assert.AreEqual(genre, revue.Genre, "devrait marcher");
            Assert.AreEqual(idPublic, revue.IdPublic, "devrait marcher");
            Assert.AreEqual(lePublic, revue.Public, "devrait marcher");
            Assert.AreEqual(idRayon, revue.IdRayon, "devrait marcher");
            Assert.AreEqual(rayon, revue.Rayon, "devrait marcher");
            Assert.AreEqual(periodicite, revue.Periodicite, "devrait marcher");
            Assert.AreEqual(delaiMiseADispo, revue.DelaiMiseADispo, "devrait marcher");
        }
    }
}
