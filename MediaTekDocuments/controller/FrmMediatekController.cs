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
    /// Contrôleur principal pour l'application MediatekDocuments
    /// Gère la communication entre la vue et l'accès aux données
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Instance d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Récupère tous les genres disponibles
        /// </summary>
        /// <returns>Liste des catégories de genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// Récupère tous les livres
        /// </summary>
        /// <returns>Liste des livres</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// Récupère tous les DVD
        /// </summary>
        /// <returns>Liste des DVD</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// Récupère toutes les revues
        /// </summary>
        /// <returns>Liste des revues</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// Récupère tous les rayons
        /// </summary>
        /// <returns>Liste des rayons</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// Récupère tous les publics cibles
        /// </summary>
        /// <returns>Liste des publics</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// Récupère tous les états de suivi
        /// </summary>
        /// <returns>Liste des suivis</returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">ID de la revue</param>
        /// <returns>Liste des exemplaires</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetExemplairesRevue(idDocument);
        }

        /// <summary>
        /// Récupère un utilisateur par son login
        /// </summary>
        /// <param name="login">Identifiant de connexion</param>
        /// <returns>Informations de l'utilisateur</returns>
        public List<Utilisateur> GetUtilisateur(string login)
        {
            return access.GetUtilisateur(login);
        }

        /// <summary>
        /// Récupère les commandes d'un document
        /// </summary>
        /// <param name="idLivreDvd">ID du document</param>
        /// <returns>Liste des commandes</returns>
        public List<CommandeDocument> GetCommandeDocument(string idLivreDvd)
        {
            return access.GetAllComDoc(idLivreDvd);
        }

        /// <summary>
        /// Récupère les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue">ID de la revue</param>
        /// <returns>Liste des abonnements</returns>
        public List<Abonnement> GetCommandeRevue(string idRevue)
        {
            return access.GetAllComRevue(idRevue);
        }

        /// <summary>
        /// Crée un nouvel exemplaire
        /// </summary>
        /// <param name="exemplaire">Objet exemplaire à créer</param>
        /// <returns>True si création réussie</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée une commande de document
        /// </summary>
        /// <param name="commandeDoc">Commande à créer</param>
        /// <returns>True si création réussie</returns>
        public bool CreerCommandeDocument(CommandeDocument commandeDoc)
        {
            return access.CreerCommandeDoc(commandeDoc);
        }

        /// <summary>
        /// Crée un abonnement
        /// </summary>
        /// <param name="abonnement">Abonnement à créer</param>
        /// <returns>True si création réussie</returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return access.CreerAbonnement(abonnement);
        }

        /// <summary>
        /// Met à jour une commande de document
        /// </summary>
        /// <param name="commandeDoc">Commande à modifier</param>
        /// <returns>True si mise à jour réussie</returns>
        public bool UpdateCommandeDocument(CommandeDocument commandeDoc)
        {
            return access.UpdateEntite("commandedocument", commandeDoc.Id, JsonConvert.SerializeObject(commandeDoc));
        }

        /// <summary>
        /// Met à jour un abonnement
        /// </summary>
        /// <param name="abonnement">Abonnement à modifier</param>
        /// <returns>True si mise à jour réussie</returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return access.UpdateEntite("abonnement", abonnement.Id, JsonConvert.SerializeObject(abonnement));
        }

        /// <summary>
        /// Supprime une commande de document
        /// </summary>
        /// <param name="commande">Commande à supprimer</param>
        /// <returns>True si suppression réussie</returns>
        public bool DeleteCommandeDoc(CommandeDocument commande)
        {
            return access.DeleteCommande(commande);
        }

        /// <summary>
        /// Supprime un abonnement
        /// </summary>
        /// <param name="abonnement">Abonnement à supprimer</param>
        /// <returns>True si suppression réussie</returns>
        public bool DeleteAbonnement(Abonnement abonnement)
        {
            return access.DeleteAbonnement(abonnement);
        }

        /// <summary>
        /// Récupère le prochain ID disponible pour les commandes
        /// </summary>
        /// <returns>ID maximum + 1</returns>
        public string GetMaxIdCommande()
        {
            return access.GetMaxIndex();
        }
    }
}