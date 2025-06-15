using System;


namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Commande de document
    /// </summary>
    public class CommandeDocument : Commande
    {
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public int IdSuivi { get; }
        public string EtapeSuivi { get; }

        public CommandeDocument(string id, DateTime dateCommande, float montant, int nbExemplaire,
            string idLivreDvd, int idSuivi, string etapeSuivi)
            : base(id, dateCommande, montant)
        {
            this.NbExemplaire = nbExemplaire;
            this.IdLivreDvd = idLivreDvd;
            this.IdSuivi = idSuivi;
            this.EtapeSuivi = etapeSuivi;
        }
    }
}
