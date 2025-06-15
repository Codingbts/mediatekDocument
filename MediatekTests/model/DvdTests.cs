using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class DvdTests
    {
        private const string id = "55555";
        private const string titre = "Naruto the last";
        private const string image = "https://fr.wikipedia.org/wiki/Naruto_the_Last%2C_le_film";
        private const int duree = 126;
        private const string realisateur = "Luc Besson";
        private const string synopsis = "Deux ans après les événements de la Quatrième Grande Guerre Shinobi, " +
            "la lune commence à descendre vers la Terre. Avec la lune qui prévoit de détruire celle-ci, Naruto" +
            " doit faire face à cette nouvelle menace. Pendant ce temps, un homme mystérieux apparaît, kidnappe" +
            " Hanabi Hyûga après avoir échoué sur Hinata, qui parvient à donner un don d'amour à Naruto au Festival" +
            " d'hiver de Konoha. Naruto, Hinata, Sakura, Saï et Shikamaru sont envoyés en mission pour aller sauver " +
            "Hanabi. Finalement, Hinata est aussi capturée et Naruto doit ainsi aller les sauver toutes les deux.";
        private const string idGenre = "00005";
        private const string genre = "science-fiction";
        private const string idPublic = "00500";
        private const string lePublic = "Tout publics";
        private const string idRayon = "cinq";
        private const string rayon = "Le Cinquième";
        private static Dvd leDvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic,
            lePublic, idRayon, rayon);
        [TestMethod()]
        public void DvdTest()
        {
            Assert.AreEqual(id, leDvd.Id, "devrait marcher");
            Assert.AreEqual(titre, leDvd.Titre, "devrait marcher");
            Assert.AreEqual(image, leDvd.Image, "devrait marcher");
            Assert.AreEqual(duree, leDvd.Duree, "devrait marcher");
            Assert.AreEqual(realisateur, leDvd.Realisateur, "devrait marcher");
            Assert.AreEqual(synopsis, leDvd.Synopsis, "devrait marcher");
            Assert.AreEqual(idGenre, leDvd.IdGenre, "devrait marcher");
            Assert.AreEqual(genre, leDvd.Genre, "devrait marcher");
            Assert.AreEqual(idPublic, leDvd.IdPublic, "devrait marcher");
            Assert.AreEqual(lePublic, leDvd.Public, "devrait marcher");
            Assert.AreEqual(idRayon, leDvd.IdRayon, "devrait marcher");
            Assert.AreEqual(rayon, leDvd.Rayon, "devrait marcher");
        }
    }
}