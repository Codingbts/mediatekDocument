using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Commande 
    /// </summary>
    public class Commande
    {
        public string Id { get; }
        public DateTime DateCommande { get; set; }

        public float Montant { get; }


        public Commande(string id, DateTime dateCommande, float montant)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.Montant = montant;

        }
    }


}