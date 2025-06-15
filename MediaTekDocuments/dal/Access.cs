using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Windows.Forms;
using System.Configuration;
using Serilog;
using System.Linq;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "MediaTekDocuments.Properties.Settings.MediatTelDocumentsURIConnectionString";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";

        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// nom de connexion à la bdd
        /// </summary>
        private static readonly string authenticationString = "MediaTekDocuments.Properties.Settings.MediatTelDocumentsConnectionString";


        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String getUriApi = GetConnectionStringByName(uriApi);
            string getAuthenticationString = GetConnectionStringByName(authenticationString);
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt")
                    .CreateLogger();
                api = ApiRest.GetInstance(getUriApi, getAuthenticationString);
            }
            catch (Exception e)
            {
                Log.Fatal("Access catch erreur={0}", e.Message);
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Récupération de la chaîne de connexion
        /// </summary>
        /// <param name="name">Nom de la chaîne de connexion</param>
        /// <returns>Valeur de la chaîne de connexion</returns>
        static string GetConnectionStringByName(string name)
        {
            string returnValue = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
                returnValue = settings.ConnectionString;
            return returnValue;
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne tous les états de suivi à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            IEnumerable<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi", null);
            return new List<Suivi>(lesSuivis);
        }

        /// <summary>
        /// Retourne tous les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne tous les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne l'index max des commandes
        /// </summary>
        /// <returns>ID maximum sous forme de chaîne</returns>
        public string GetMaxIndex()
        {
            List<Categorie> maxindex = TraitementRecup<Categorie>(GET, "commandemax", null);
            return maxindex[0].Id;
        }

        /// <summary>
        /// Retourne toutes les commandes d'un livre ou DVD
        /// </summary>
        /// <param name="idLivreDvd">ID du livre ou DVD</param>
        /// <returns>Liste de commandes</returns>
        public List<CommandeDocument> GetAllComDoc(string idLivreDvd)
        {
            String jsonIdLivreDvd = convertToJson("idLivreDvd", idLivreDvd);
            List<CommandeDocument> lesComDoc = TraitementRecup<CommandeDocument>(GET, "commandedocument/" + jsonIdLivreDvd, null);
            return lesComDoc;
        }

        /// <summary>
        /// Retourne tous les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue">ID de la revue</param>
        /// <returns>Liste d'abonnements</returns>
        public List<Abonnement> GetAllComRevue(string idRevue)
        {
            String jsonIdRevue = convertToJson("idRevue", idRevue);
            List<Abonnement> lesComRevue = TraitementRecup<Abonnement>(GET, "abonnement/" + jsonIdRevue, null);
            return lesComRevue;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Retourne un utilisateur par son login
        /// </summary>
        /// <param name="login">Login de l'utilisateur</param>
        /// <returns>Informations de l'utilisateur</returns>
        public List<Utilisateur> GetUtilisateur(string login)
        {
            String jsonLogin = convertToJson("login", login);
            List<Utilisateur> recupUtili = TraitementRecup<Utilisateur>(GET, "utilisateur/" + jsonLogin, null);
            return recupUtili;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la création de l'exemplaire : {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// ecriture d'une commande de document en base de données
        /// </summary>
        /// <param name="commandeDoc">Commande du document à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerCommandeDoc(CommandeDocument commandeDoc)
        {
            String jsonCommandeDoc = JsonConvert.SerializeObject(commandeDoc, new CustomDateTimeConverter());
            try
            {
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument", "champs=" + jsonCommandeDoc);
                Console.WriteLine("Réponse du serveur : " + JsonConvert.SerializeObject(liste));
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la creation du document : {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Crée un nouvel abonnement en base de données
        /// </summary>
        /// <param name="abonnement">Abonnement à créer</param>
        /// <returns>true si la création a réussi</returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            String jsonAbonnement = JsonConvert.SerializeObject(abonnement, new CustomDateTimeConverter());
            try
            {
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement", "champs=" + jsonAbonnement);
                Console.WriteLine("Réponse du serveur : " + JsonConvert.SerializeObject(liste));
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la création de l'abonnement: {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifie une entité dans la base de données
        /// </summary>
        /// <param name="type">Type d'entité</param>
        /// <param name="id">ID de l'entité</param>
        /// <param name="jsonEntite">Données au format JSON</param>
        /// <returns>true si la modification a réussi</returns>
        public bool UpdateEntite(string type, string id, string jsonEntite)
        {
            try
            {
                string encodedContent = Uri.EscapeDataString(jsonEntite);
                string fullBody = "champs=" + encodedContent;
                List<Object> liste = TraitementRecup<Object>(PUT, type + "/" + id, fullBody);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la mise à jour de l'entité : {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprime une commande
        /// </summary>
        /// <param name="comm">Commande à supprimer</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool DeleteCommande(CommandeDocument comm)
        {
            try
            {
                String jsonCommandeDelete = JsonConvert.SerializeObject(comm);
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, "commandedocument" + "/" + jsonCommandeDelete, null);
                Console.WriteLine("Réponse du serveur : " + JsonConvert.SerializeObject(liste));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la suppression de la commande : {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Supprime un abonnement
        /// </summary>
        /// <param name="abo">Abonnement à supprimer</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool DeleteAbonnement(Abonnement abo)
        {
            try
            {
                String jsonAbonnementDelete = JsonConvert.SerializeObject(abo);
                List<Abonnement> liste = TraitementRecup<Abonnement>(DELETE, "abonnement" + "/" + jsonAbonnementDelete, null);
                Console.WriteLine("Réponse du serveur : " + JsonConvert.SerializeObject(liste));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Erreur lors de la suppression de l'abonnement : {Message}", ex.Message);
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T">Type d'objet</typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Log.Error("Erreur lors de l'accès à l'API : {Message}", e.Message);
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom">Nom de la propriété</param>
        /// <param name="valeur">Valeur de la propriété</param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}