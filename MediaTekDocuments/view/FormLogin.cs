using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    public partial class FormLogin : Form
    {
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const int Iterations = 100_000;
        private FrmMediatekController controller;

        public FormLogin()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        private void verifConnexion()
        {
            string loginEcrit = txtLogin.Text;
            string passwordEcrit = txtPassword.Text;
            List<Utilisateur> utilisateur = controller.GetUtilisateur(loginEcrit);

            if(!string.IsNullOrEmpty(loginEcrit) && utilisateur.Count == 0)
            {
                MessageBox.Show("Le login est incorrect ! Veuillez réessayer");
            }


            if (string.IsNullOrEmpty(loginEcrit) && string.IsNullOrEmpty(passwordEcrit))
            {
                MessageBox.Show("Aucun champ n'a été rempli !");
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(loginEcrit))
            {
                MessageBox.Show("Le login est manquant, veuillez réessayer.");
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passwordEcrit))
            {
                MessageBox.Show("Le mot de passe est manquant est manquant, veuillez réessayer.");
                txtLogin.Focus();
                return;
            }

            foreach (Utilisateur utili in utilisateur)
            {
                string loginBDD = utili.Login;
                string passwordBdd = utili.Password;
                try
                {
                    if(loginEcrit == loginBDD)
                    {
                        if(VerifPassword(passwordEcrit, passwordBdd))
                        {
                            MessageBox.Show("Connexion réussi.");
                            this.Hide();
                            string serviceUtili = utili.Service;
                            VerifDroitAccesApp(serviceUtili);
                        }
                        else
                        {
                            MessageBox.Show("le mot de passe est incorrect, veuillez réessayer.");
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
            
               
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                byte[] hashBytes = new byte[SaltSize + HashSize];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, HashSize);


                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifPassword(string passwordEntrer, string passwordBDD)
        {
            byte[] hashBytes = Convert.FromBase64String(passwordBDD);

            byte[] recupSalt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, recupSalt, 0, SaltSize);

            byte[] recupHash = new byte[HashSize];
            Buffer.BlockCopy(hashBytes, SaltSize, recupHash, 0, HashSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(passwordEntrer, recupSalt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] Hash = pbkdf2.GetBytes(HashSize);
                return HashCompare(Hash, recupHash);

            }
        }

        private bool HashCompare(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            int result = 0;
            for (int i = 0; i < hash1.Length; i++)
            {
                result = result | hash1[i] ^ hash2[i];
            }
            return result == 0;
        }

        private void btnValidForm_Click(object sender, EventArgs e)
        {
            verifConnexion();
        }

        private void VerifDroitAccesApp(string serviceUtili)
        {

            switch (serviceUtili)
            {
                case "administrateur":
                    FrmMediatek mediatekAdmin = new FrmMediatek(serviceUtili);
                    mediatekAdmin.Show();
                break;
                case "administratif":
                    FrmMediatek mediatekAdministratif = new FrmMediatek(serviceUtili);
                    mediatekAdministratif.Show();
                    break;
                case "pret":
                    FrmMediatek mediatekPret = new FrmMediatek(serviceUtili);
                    mediatekPret.Show();
                    break;
                case "culture":
                    MessageBox.Show("Vos droits ne sont pas suffisant pour accéder à cette application");
                    Application.Exit();
                    break;
                default:
                    MessageBox.Show("Aucun service ne vous est attribué");
                    break;
            }

        }
    }
}
