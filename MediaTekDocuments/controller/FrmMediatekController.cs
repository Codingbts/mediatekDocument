using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        ///<summary>
        /// getter sur les suivis
        ///</summary>
        ///<returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetExemplairesRevue(idDocument);
        }

        /// <summary>
        /// récupère les commandes d'un document
        /// </summary>
        /// <param name="idLivreDvd">id du document concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<CommandeDocument> GetCommandeDocument(string idLivreDvd)
        {
            return access.GetAllComDoc(idLivreDvd);
        }

        /// <summary>
        /// récupère les commandes d'une revue
        /// </summary>
        /// <param name="idRevue">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Abonnement> GetCommandeRevue(string idRevue)
        {
            return access.GetAllComRevue(idRevue);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée une commande de document dans la bdd
        /// </summary>
        /// <param name="commandeDoc">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerCommandeDocument( CommandeDocument commandeDoc)
        {
            return access.CreerCommandeDoc(commandeDoc);
        }

        /// <summary>
        /// Crée une commande de revye dans la bdd
        /// </summary>
        /// <param name="commandeDoc">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return access.CreerAbonnement(abonnement);
        }

        // <summary>
        /// Modifier une commande de document dans la bdd
        /// </summary>
        /// <param name="commandeDoc">L'objet commandedocument concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool UpdateCommandeDocument(CommandeDocument commandeDoc)
        {
            return access.UpdateEntite("commandedocument", commandeDoc.Id, JsonConvert.SerializeObject(commandeDoc));
        }

        // <summary>
        /// Modifier une commande de revue dans la bdd
        /// </summary>
        /// <param name="commandeDoc">L'objet commandedocument concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return access.UpdateEntite("abonnement", abonnement.Id, JsonConvert.SerializeObject(abonnement));
        }

        public bool DeleteCommandeDoc(CommandeDocument commande)
        {
            return access.DeleteCommande(commande);
        }

        public bool DeleteAbonnement(Abonnement abonnement)
        {
            return access.DeleteAbonnement(abonnement);
        }
        public string GetMaxIdCommande()
        {
            return access.GetMaxIndex();
        }

    }
}
