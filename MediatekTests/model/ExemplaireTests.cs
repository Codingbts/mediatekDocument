using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.model.Tests
{
    [TestClass]
    public class ExemplaireTests
    {
        [TestMethod]
        public void ExemplaireTest()
        {
            int numero = 1;
            DateTime dateAchat = new DateTime(2023, 5, 1);
            string photo = "photo1.jpg";
            string idEtat = "00001";
            string idDocument = "10001";

            Exemplaire ex = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);

            Assert.AreEqual(numero, ex.Numero, "devrait marcher");
            Assert.AreEqual(dateAchat, ex.DateAchat, "devrait marcher");
            Assert.AreEqual(photo, ex.Photo, "devrait marcher");
            Assert.AreEqual(idEtat, ex.IdEtat, "devrait marcher");
            Assert.AreEqual(idDocument, ex.Id, "devrait marcher");
        }
    }
}
