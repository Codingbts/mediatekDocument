﻿using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgSuivis = new BindingSource();
        private bool ajouterBool = false;
        private bool supprComLivre = false;
        private bool supprComDVD = false;
        private bool supprComRevue = false;
        private bool bloquerGesEtape = false;
        private int ancienneEtapeSuivi;
        private bool modifBool = false;
        private bool msgBoxSuivi = true;
        private string serviceUtili;




        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek(string serviceUtili)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.serviceUtili = serviceUtili;
            VerifTempsAbo();
        }

        private void tabOnglets_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (serviceUtili == "pret")
            {
                if (e.TabPage == tabComLivres || e.TabPage == tabComDvd || e.TabPage == tabComRevues || e.TabPage == tabReceptionRevue)
                {
                    MessageBox.Show("Vos droits ne sont pas suffisant pour accéder à ces onglets !");
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
            
        }

        /// <summary>
        /// Rempli le combo box des suivi
        /// </summary>
        /// <param name="lesSuivis">liste des objets de type Suivi</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        public string IdMaxPlusUn(string id)
        {
            int taille = id.Length;
            int idnum = int.Parse(id) + 1;
            id = idnum.ToString();
            if (id.Length > taille)
                MessageBox.Show("Taille du registre arrivé a saturation");
            while (id.Length != taille)
            {
                id = "0" + id;
            }
            return id;

        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdRayons.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxComDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvComDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region CommandeLivres

        private readonly BindingSource bdgComLivresListe = new BindingSource();
        private readonly BindingSource bdgComUnLivre = new BindingSource();
        private List<Livre> lesComLivres = new List<Livre>();

        /// <summary>
        /// Événement déclenché lors de l'entrée dans l'onglet Commande de Livres
        /// Initialise les listes et combos nécessaires à la gestion des commandes
        /// </summary>
        private void TabComLivres_Enter(object sender, EventArgs e)
        {
            lesComLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComLivresRayons);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgSuivis, cbxLivresComEtapeSuivi);
            ModifComLivre(true);
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Événement déclenché lors du clic sur l'onglet Commande de Livres
        /// Réinitialise l'indicateur de message de suivi
        /// </summary>
        private void tabComLivres_Click(object sender, EventArgs e)
        {
            msgBoxSuivi = false;
        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des livres fournie
        /// Configure les colonnes visibles et leur ordre d'affichage
        /// </summary>
        private void RemplirComLivresListe(List<Livre> Comlivres)
        {
            bdgComLivresListe.DataSource = Comlivres;
            dgvComLivresListe.DataSource = bdgComLivresListe;
            dgvComLivresListe.Columns["isbn"].Visible = false;
            dgvComLivresListe.Columns["idRayon"].Visible = false;
            dgvComLivresListe.Columns["idGenre"].Visible = false;
            dgvComLivresListe.Columns["idPublic"].Visible = false;
            dgvComLivresListe.Columns["image"].Visible = false;
            dgvComLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComLivresListe.Columns["id"].DisplayIndex = 0;
            dgvComLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Affiche la liste complète des livres et réinitialise les filtres
        /// </summary>
        private void RemplirComLivresListeComplete()
        {
            RemplirComLivresListe(lesComLivres);
            VideComLivresZones();
        }

        /// <summary>
        /// Réinitialise les zones de recherche et de filtre
        /// </summary>
        private void VideComLivresZones()
        {
            cbxComLivresGenres.SelectedIndex = -1;
            cbxComLivresRayons.SelectedIndex = -1;
            cbxComLivresPublics.SelectedIndex = -1;
            txbComLivresNumRecherche.Text = "";
            txbComLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Recherche un livre par son numéro et l'affiche
        /// Affiche un message si le numéro n'est pas trouvé
        /// </summary>
        private void btnComLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComLivresNumRecherche.Text.Equals(""))
            {
                txbComLivresTitreRecherche.Text = "";
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                Livre livre = lesComLivres.Find(x => x.Id.Equals(txbComLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirComLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComLivresListeComplete();
                }
            }
            else
            {
                RemplirComLivresListeComplete();
            }
        }

        /// <summary>
        /// Filtre dynamique des livres par titre au fur et à mesure de la saisie
        /// </summary>
        private void TxbComLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComLivresTitreRecherche.Text.Equals(""))
            {
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                txbComLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesComLivres.FindAll(x => x.Titre.ToLower().Contains(txbComLivresTitreRecherche.Text.ToLower()));
                RemplirComLivresListe(lesLivresParTitre);
            }
            else
            {
                if (cbxComLivresGenres.SelectedIndex < 0 && cbxComLivresPublics.SelectedIndex < 0 && cbxComLivresRayons.SelectedIndex < 0
                    && txbComLivresNumRecherche.Text.Equals(""))
                {
                    RemplirComLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affiche les informations détaillées du livre sélectionné
        /// </summary>
        private void AfficheComLivresInfos(Livre livre)
        {
            txbInfoLivresAuteur.Text = livre.Auteur;
            txbInfoLivresCollection.Text = livre.Collection;
            txbInfoLivresIsbn.Text = livre.Isbn;
            txbInfoLivresNumero.Text = livre.Id;
            txbInfoLivresGenre.Text = livre.Genre;
            txbInfoLivresPublic.Text = livre.Public;
            txbInfoLivresRayon.Text = livre.Rayon;
            txbInfoLivresTitre.Text = livre.Titre;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des informations du livre
        /// </summary>
        private void VideComLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
        }

        /// <summary>
        /// Filtre les livres par genre sélectionné
        /// </summary>
        private void CbxComLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresGenres.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxComLivresGenres.SelectedItem;
                List<Livre> livres = lesComLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComLivresListe(livres);
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les livres par public sélectionné
        /// </summary>
        private void CbxComLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresPublics.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxComLivresPublics.SelectedItem;
                List<Livre> livres = lesComLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComLivresListe(livres);
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les livres par rayon sélectionné
        /// </summary>
        private void CbxComLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresRayons.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxComLivresRayons.SelectedItem;
                List<Livre> livres = lesComLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComLivresListe(livres);
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Événement de sélection d'un livre dans la liste
        /// Affiche les informations du livre et ses commandes associées
        /// </summary>
        private void DgvComLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgComLivresListe.List[bdgComLivresListe.Position];
                    txbLivresComNumLivre.Text = livre.Id;
                    AfficheComLivresInfos(livre);

                    List<CommandeDocument> comDoc = controller.GetCommandeDocument(livre.Id);
                    RemplirComLivresDetails(comDoc);

                    if (ajouterBool)
                    {
                        ModifComLivre(true);
                        ajouterBool = false;
                        if (comDoc.Count == 0)
                        {
                            txbLivresComNbCommande.Text = "";
                        }
                    }

                    if (supprComLivre)
                    {
                        supprComLivre = false;
                        List<CommandeDocument> comDocSuppr = controller.GetCommandeDocument(livre.Id);
                        RemplirComLivresDetails(comDocSuppr);
                        ModifComLivre(true);
                    }

                    if (modifBool)
                    {
                        modifBool = false;
                        List<CommandeDocument> comDocSuppr = controller.GetCommandeDocument(livre.Id);
                        RemplirComLivresDetails(comDocSuppr);
                        ModifComLivre(true);
                    }
                }
                catch
                {
                    VideComLivresZones();
                }
            }
            else
            {
                VideComLivresInfos();
            }
        }

        /// <summary>
        /// Annule le filtre par genre
        /// </summary>
        private void btnComLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Annule le filtre par public
        /// </summary>
        private void btnComLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Annule le filtre par rayon
        /// </summary>
        private void btnComLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Trie les livres selon la colonne cliquée
        /// </summary>
        private void DgvComLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComLivresZones();
            string titreColonne = dgvComLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesComLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesComLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesComLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComLivresListe(sortedList);
        }

        /// <summary>
        /// Remplit la liste des commandes pour le livre sélectionné
        /// </summary>
        private void RemplirComLivresDetails(List<CommandeDocument> Comlivre)
        {
            bdgComUnLivre.DataSource = Comlivre;
            dgvComLivreUn.DataSource = bdgComUnLivre;
            dgvComLivreUn.Columns["idLivreDvd"].Visible = false;
            dgvComLivreUn.Columns["idSuivi"].Visible = false;
            dgvComLivreUn.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComLivreUn.Columns["dateCommande"].DisplayIndex = 0;
        }

        /// <summary>
        /// Affiche les détails de la commande sélectionnée
        /// </summary>
        private void AfficheComLivreUn(CommandeDocument Comlivre, Livre livre)
        {
            txbLivresComNbCommande.Text = Comlivre.Id;
            dtpLivresComDateCommande.Value = Comlivre.DateCommande;
            txbLivresComMontant.Text = Comlivre.Montant.ToString();
            txbLivresComNbExemplaires.Text = Comlivre.NbExemplaire.ToString();
            if (Comlivre.IdLivreDvd == "")
            {
                txbLivresComNumLivre.Text = livre.Id;
            }
            else
            {
                txbLivresComNumLivre.Text = Comlivre.IdLivreDvd;
            }

            bloquerGesEtape = true;
            cbxLivresComEtapeSuivi.SelectedIndexChanged -= cbxLivresComEtapeSuivi_SelectedIndexChanged;
            cbxLivresComEtapeSuivi.SelectedIndex = cbxLivresComEtapeSuivi.FindString(Comlivre.EtapeSuivi);
            ancienneEtapeSuivi = cbxLivresComEtapeSuivi.SelectedIndex;
            cbxLivresComEtapeSuivi.SelectedIndexChanged += cbxLivresComEtapeSuivi_SelectedIndexChanged;
            bloquerGesEtape = false;
        }

        /// <summary>
        /// Active/désactive les contrôles pour l'édition d'une commande
        /// </summary>
        private void ModifComLivre(bool modif)
        {
            txbLivresComNbCommande.Enabled = false;
            dtpLivresComDateCommande.Enabled = !modif;
            txbLivresComMontant.Enabled = !modif;
            txbLivresComNbExemplaires.Enabled = !modif;
            txbLivresComNumLivre.Enabled = false;
            cbxLivresComEtapeSuivi.Enabled = !modif;
            btnLivresComAjouter.Enabled = modif;
            btnLivresComModifier.Enabled = modif;
            btnLivresComSupprimer.Enabled = modif;
            btnLivresComAnnuler.Enabled = !modif;
            btnLivresComValider.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// Événement de sélection d'une commande dans la liste
        /// </summary>
        private void DgvComLivreUn_SelectionChanged(object sender, EventArgs e)
        {
            bloquerGesEtape = true;

            if (dgvComLivreUn.CurrentCell != null)
            {
                try
                {
                    CommandeDocument comLivre = (CommandeDocument)bdgComUnLivre.List[bdgComUnLivre.Position];
                    AfficheComLivreUn(comLivre, null);
                }
                catch
                {
                    VideComLivresZones();
                }
            }
            else
            {
                VideComLivreUnInfos();
            }

            bloquerGesEtape = false;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des détails de commande
        /// </summary>
        private void VideComLivreUnInfos()
        {
            if (!ajouterBool)
            {
                txbLivresComNbCommande.Text = "";
            }
            dtpLivresComDateCommande.MaxDate = DateTime.Today;
            txbLivresComMontant.Text = "";
            txbLivresComNbExemplaires.Text = "";
            cbxLivresComEtapeSuivi.SelectedIndex = -1;
        }

        /// <summary>
        /// Crée ou met à jour une commande selon le contexte
        /// </summary>
        private void AjoutUpCommandeLivre(CommandeDocument commandeDocument)
        {
            if (ajouterBool)
            {
                bool commandeCreer = controller.CreerCommandeDocument(commandeDocument);
                MessageBox.Show("Ajout de la nouvelle commande");

                if (!commandeCreer)
                {
                    MessageBox.Show("Erreur lors de la création de la commande");
                }
            }
            else if (!ajouterBool)
            {
                try
                {
                    bloquerGesEtape = true;
                    controller.UpdateCommandeDocument(commandeDocument);
                    MessageBox.Show("Modification de la commande effectué");
                }
                catch
                {
                    MessageBox.Show("Erreur lors de la modification de la commande");
                }
            }
        }

        /// <summary>
        /// Événement de clic sur le bouton d'ajout de commande
        /// </summary>
        private void btnLivresComAjouter_Click(object sender, EventArgs e)
        {
            msgBoxSuivi = false;
            ModifComLivre(false);
            ajouterBool = true;
            VideComLivreUnInfos();
            bloquerGesEtape = true;
            cbxLivresComEtapeSuivi.SelectedIndexChanged -= cbxLivresComEtapeSuivi_SelectedIndexChanged;
            try
            {
                cbxLivresComEtapeSuivi.SelectedIndex = 0;
            }
            catch
            {
                return;
            }
            cbxLivresComEtapeSuivi.SelectedIndexChanged += cbxLivresComEtapeSuivi_SelectedIndexChanged;
            cbxLivresComEtapeSuivi.Enabled = false;
            dtpLivresComDateCommande.Enabled = false;
            msgBoxSuivi = true;

            try
            {
                string maxId = controller.GetMaxIdCommande();

                if (string.IsNullOrEmpty(maxId))
                {
                    MessageBox.Show("Erreur : aucun ID existant trouvé");
                    maxId = "0";
                }

                string newId = IdMaxPlusUn(maxId);

                if (!string.IsNullOrEmpty(newId))
                {
                    txbLivresComNbCommande.Text = newId;
                }
                else
                {
                    MessageBox.Show("Erreur lors de la génération du nouvel ID");
                    txbLivresComNbCommande.Text = "00001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur critique : {ex.Message}");
                txbLivresComNbCommande.Text = "00001";
            }
            bloquerGesEtape = false;
        }

        /// <summary>
        /// Événement de clic sur le bouton de modification de commande
        /// </summary>
        private void btnLivresComModifier_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            modifBool = true;
            ModifComLivre(false);
        }

        /// <summary>
        /// Gère le changement d'étape de suivi avec validation des transitions
        /// </summary>
        private void GestionCbxSuivi(int nvEtape)
        {
            if (bloquerGesEtape)
            {
                return;
            }

            if (nvEtape == ancienneEtapeSuivi)
            {
                return;
            }
            if (nvEtape < ancienneEtapeSuivi && ancienneEtapeSuivi >= 2)
            {
                if (msgBoxSuivi)
                {
                    MessageBox.Show("Il est impossible de sélectionner une étape antérieure à l'étape actuel.");
                    cbxLivresComEtapeSuivi.SelectedIndex = ancienneEtapeSuivi;
                    return;
                }
            }

            if (ancienneEtapeSuivi < 2 && nvEtape == 3)
            {
                MessageBox.Show("Il faut que la commande soit livrée pour être réglée !");
                cbxLivresComEtapeSuivi.SelectedIndex = ancienneEtapeSuivi;
                return;
            }
            ancienneEtapeSuivi = nvEtape;
        }

        /// <summary>
        /// Événement de clic sur le bouton de validation
        /// Gère la validation selon le contexte (ajout/modification/suppression)
        /// </summary>
        private void btnLivresComValider_Click(object sender, EventArgs e)
        {
            bloquerGesEtape = true;

            if (supprComLivre)
            {
                if (MessageBox.Show("Etes vous sur de vouloir supprimer cette commande ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommandeDocument comListeDelete = (CommandeDocument)bdgComUnLivre.List[bdgComUnLivre.Position];
                    if (dgvComLivreUn.CurrentCell != null && txbLivresComNbCommande.Text != "")
                    {
                        if (comListeDelete.IdSuivi <= 2)
                        {
                            bool resultDelete = controller.DeleteCommandeDoc(comListeDelete);
                            if (resultDelete)
                            {
                                MessageBox.Show("Commande supprimé");
                                VideComLivreUnInfos();
                                List<CommandeDocument> comListMaj = controller.GetCommandeDocument(comListeDelete.IdLivreDvd);
                                RemplirComLivresDetails(comListMaj);
                                supprComLivre = false;
                            }
                            else
                            {
                                MessageBox.Show("Erreur lors de la suppression de la commande");
                            }
                            supprComLivre = false;
                        }
                        else
                        {
                            MessageBox.Show("Vous ne pouvez pas supprimer une commande livrée ou réglée !");
                            supprComLivre = false;
                        }
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Etes vous sur de faire cette action ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string id = txbLivresComNbCommande.Text;
                    DateTime dateCommande = dtpLivresComDateCommande.Value;
                    float montant = float.TryParse(txbLivresComMontant.Text, out _) ? float.Parse(txbLivresComMontant.Text) : -1;
                    int nbExemplaire = int.TryParse(txbLivresComNbExemplaires.Text, out _) ? int.Parse(txbLivresComNbExemplaires.Text) : -1;
                    string idLivreDvd = txbLivresComNumLivre.Text;
                    int idSuivi = 0;
                    string etapeSuivi = "";
                    Suivi suivi = (Suivi)cbxLivresComEtapeSuivi.SelectedItem;
                    VerifValueLivreCom();
                    VideComLivreUnInfos();
                    if (suivi != null)
                    {
                        idSuivi = suivi.IdSuivi;
                        etapeSuivi = suivi.EtapeSuivi;
                    }
                    if (montant != -1 && nbExemplaire != -1 && etapeSuivi != "")
                    {
                        CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etapeSuivi);
                        AjoutUpCommandeLivre(commandeLivre);
                        if (ajouterBool)
                        {
                            ajouterBool = false;
                        }
                        List<CommandeDocument> comListMaj = controller.GetCommandeDocument(idLivreDvd);
                        RemplirComLivresDetails(comListMaj);
                    }
                }
            }
            ModifComLivre(true);
        }

        /// <summary>
        /// Vérifie la validité des valeurs saisies pour une commande
        /// </summary>
        private void VerifValueLivreCom()
        {
            float montant = -1;
            int nbExemplaire = -1;

            try
            {
                montant = float.Parse(txbLivresComMontant.Text);
                nbExemplaire = int.Parse(txbLivresComNbExemplaires.Text);
            }
            catch
            {
                if (montant == -1)
                {
                    MessageBox.Show("Le montant doit comporter une virgule !");
                }

                if (nbExemplaire == -1)
                {
                    MessageBox.Show("Le nombre d'exemplaires doit être un nombre entier !");
                }
            }
            Suivi suivi = (Suivi)cbxLivresComEtapeSuivi.SelectedItem;
            if (suivi == null)
            {
                MessageBox.Show("Vous devez selectionner une étape");
            }
        }

        /// <summary>
        /// Événement de clic sur le bouton d'annulation
        /// </summary>
        private void btnLivresComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            ModifComLivre(true);
            bloquerGesEtape = true;
            Livre livreRecupId = (Livre)bdgComLivresListe.List[bdgComLivresListe.Position];
            List<CommandeDocument> comListMaj = controller.GetCommandeDocument(livreRecupId.Id);
            RemplirComLivresDetails(comListMaj);
        }

        /// <summary>
        /// Événement de clic sur le bouton de suppression de commande
        /// </summary>
        private void btnLivresComSupprimer_Click(object sender, EventArgs e)
        {
            supprComLivre = true;
            ModifComLivre(true);
            btnLivresComAjouter.Enabled = false;
            btnLivresComModifier.Enabled = false;
            btnLivresComAnnuler.Enabled = true;
            btnLivresComValider.Enabled = true;
        }

        /// <summary>
        /// Événement de changement de sélection dans la liste des étapes de suivi
        /// </summary>
        private void cbxLivresComEtapeSuivi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bloquerGesEtape)
            {
                return;
            }

            int nvEtape = cbxLivresComEtapeSuivi.SelectedIndex;
            GestionCbxSuivi(nvEtape);
        }

        #endregion

        #region Onglet CommandeDvd
        private readonly BindingSource bdgComDvdListe = new BindingSource();
        private readonly BindingSource bdgComUnDVD = new BindingSource();
        private List<Dvd> lesComDvd = new List<Dvd>();

        /// <summary>
        /// Événement déclenché lors de l'entrée dans l'onglet Commande de DVD
        /// Initialise les listes et combos nécessaires à la gestion des commandes
        /// </summary>
        private void tabComDvd_Enter(object sender, EventArgs e)
        {
            lesComDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComDvdRayons);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgSuivis, cbxDVDComEtapeSuivi);
            RemplirComDvdListeComplete();
            ModifComDVD(true);
            bloquerGesEtape = true;
        }

        /// <summary>
        /// Événement déclenché lors du clic sur l'onglet Commande de DVD
        /// Réinitialise l'indicateur de message de suivi
        /// </summary>
        private void tabComDVD_Click(object sender, EventArgs e)
        {
            msgBoxSuivi = false;
        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des DVD fournie
        /// Configure les colonnes visibles et leur ordre d'affichage
        /// </summary>
        private void RemplirComDvdListe(List<Dvd> Dvds)
        {
            bdgComDvdListe.DataSource = Dvds;
            dgvComDvdListe.DataSource = bdgComDvdListe;
            dgvComDvdListe.Columns["idRayon"].Visible = false;
            dgvComDvdListe.Columns["idGenre"].Visible = false;
            dgvComDvdListe.Columns["idPublic"].Visible = false;
            dgvComDvdListe.Columns["image"].Visible = false;
            dgvComDvdListe.Columns["synopsis"].Visible = false;
            dgvComDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComDvdListe.Columns["id"].DisplayIndex = 0;
            dgvComDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche un DVD par son numéro et l'affiche
        /// Affiche un message si le numéro n'est pas trouvé
        /// </summary>
        private void btnComDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComDvdNumRecherche.Text.Equals(""))
            {
                txbComDvdTitreRecherche.Text = "";
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesComDvd.Find(x => x.Id.Equals(txbComDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirComDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComDvdListeComplete();
                }
            }
            else
            {
                RemplirComDvdListeComplete();
            }
        }

        /// <summary>
        /// Filtre dynamique des DVD par titre au fur et à mesure de la saisie
        /// </summary>
        private void txbComDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComDvdTitreRecherche.Text.Equals(""))
            {
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                txbComDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesComDvd.FindAll(x => x.Titre.ToLower().Contains(txbComDvdTitreRecherche.Text.ToLower()));
                RemplirComDvdListe(lesDvdParTitre);
            }
            else
            {
                if (cbxComDvdGenres.SelectedIndex < 0 && cbxComDvdPublics.SelectedIndex < 0 && cbxComDvdRayons.SelectedIndex < 0
                    && txbComDvdNumRecherche.Text.Equals(""))
                {
                    RemplirComDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affiche les informations détaillées du DVD sélectionné
        /// </summary>
        private void AfficheComDvdInfos(Dvd dvd)
        {
            txbInfoDvdRealisateur.Text = dvd.Realisateur;
            txbInfoDvdSynopsis.Text = dvd.Synopsis;
            txbInfoDvdDuree.Text = dvd.Duree.ToString();
            txbInfoDvdNumero.Text = dvd.Id;
            txbInfoDvdGenres.Text = dvd.Genre;
            txbInfoDvdPublics.Text = dvd.Public;
            txbInfoDvdRayons.Text = dvd.Rayon;
            txbInfoDvdTitre.Text = dvd.Titre;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des informations du DVD
        /// </summary>
        private void VideComDvdInfos()
        {
            txbInfoDvdRealisateur.Text = "";
            txbInfoDvdSynopsis.Text = "";
            txbInfoDvdDuree.Text = "";
            txbInfoDvdNumero.Text = "";
            txbInfoDvdGenres.Text = "";
            txbInfoDvdPublics.Text = "";
            txbInfoDvdRayons.Text = "";
            txbInfoDvdTitre.Text = "";
        }

        /// <summary>
        /// Filtre les DVD par genre sélectionné
        /// </summary>
        private void cbxComDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdGenres.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxComDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les DVD par public sélectionné
        /// </summary>
        private void cbxComDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdPublics.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxComDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les DVD par rayon sélectionné
        /// </summary>
        private void cbxComDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdRayons.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxComDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Événement de sélection d'un DVD dans la liste
        /// Affiche les informations du DVD et ses commandes associées
        /// </summary>
        private void dgvComdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
                    txbDVDComNumDVD.Text = dvd.Id;
                    AfficheComDvdInfos(dvd);

                    List<CommandeDocument> comDoc = controller.GetCommandeDocument(dvd.Id);
                    RemplirComDVDDetails(comDoc);

                    if (ajouterBool)
                    {
                        ModifComDVD(true);
                        ajouterBool = false;
                        if (comDoc.Count == 0)
                        {
                            txbDVDComNbCommande.Text = "";
                        }
                    }

                    if (supprComDVD)
                    {
                        supprComDVD = false;
                        List<CommandeDocument> comDocSuppr = controller.GetCommandeDocument(dvd.Id);
                        RemplirComDVDDetails(comDocSuppr);
                        ModifComDVD(true);
                    }

                    if (modifBool)
                    {
                        modifBool = false;
                        List<CommandeDocument> comDocSuppr = controller.GetCommandeDocument(dvd.Id);
                        RemplirComDVDDetails(comDocSuppr);
                        ModifComDVD(true);
                    }
                }
                catch
                {
                    VideComDvdZones();
                }
            }
            else
            {
                VideComDvdInfos();
            }
        }

        /// <summary>
        /// Annule le filtre par public et réaffiche la liste complète
        /// </summary>
        private void btnComDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Annule le filtre par rayon et réaffiche la liste complète
        /// </summary>
        private void btnComDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Annule le filtre par genre et réaffiche la liste complète
        /// </summary>
        private void btnComDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Affiche la liste complète des DVD et réinitialise les filtres
        /// </summary>
        private void RemplirComDvdListeComplete()
        {
            RemplirComDvdListe(lesComDvd);
            VideComDvdZones();
        }

        /// <summary>
        /// Réinitialise les zones de recherche et de filtre
        /// </summary>
        private void VideComDvdZones()
        {
            cbxComDvdGenres.SelectedIndex = -1;
            cbxComDvdRayons.SelectedIndex = -1;
            cbxComDvdPublics.SelectedIndex = -1;
            txbComDvdNumRecherche.Text = "";
            txbComDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Trie les DVD selon la colonne cliquée
        /// </summary>
        private void dgvComDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComDvdZones();
            string titreColonne = dgvComDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesComDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesComDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesComDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComDvdListe(sortedList);
        }

        /// <summary>
        /// Remplit la liste des commandes pour le DVD sélectionné
        /// </summary>
        private void RemplirComDVDDetails(List<CommandeDocument> Comdvd)
        {
            bdgComUnDVD.DataSource = Comdvd;
            dgvComDVDUn.DataSource = bdgComUnDVD;
            dgvComDVDUn.Columns["idLivreDvd"].Visible = false;
            dgvComDVDUn.Columns["idSuivi"].Visible = false;
            dgvComDVDUn.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComDVDUn.Columns["dateCommande"].DisplayIndex = 0;
        }

        /// <summary>
        /// Affiche les détails de la commande sélectionnée
        /// </summary>
        private void AfficheComDVDUn(CommandeDocument Comdvd, Dvd dvd)
        {
            txbDVDComNbCommande.Text = Comdvd.Id;
            dtpDVDComDateCommande.Value = Comdvd.DateCommande;
            txbDVDComMontant.Text = Comdvd.Montant.ToString();
            txbDVDComNbExemplaires.Text = Comdvd.NbExemplaire.ToString();
            if (Comdvd.IdLivreDvd == "")
            {
                txbDVDComNumDVD.Text = dvd.Id;
            }
            else
            {
                txbDVDComNumDVD.Text = Comdvd.IdLivreDvd;
            }
            bloquerGesEtape = true;
            cbxDVDComEtapeSuivi.SelectedIndexChanged -= cbxDVDComEtapeSuivi_SelectedIndexChanged;
            cbxDVDComEtapeSuivi.SelectedIndex = cbxDVDComEtapeSuivi.FindString(Comdvd.EtapeSuivi);
            ancienneEtapeSuivi = cbxDVDComEtapeSuivi.SelectedIndex;
            cbxDVDComEtapeSuivi.SelectedIndexChanged += cbxDVDComEtapeSuivi_SelectedIndexChanged;
            bloquerGesEtape = false;
        }

        /// <summary>
        /// Active/désactive les contrôles pour l'édition d'une commande
        /// </summary>
        private void ModifComDVD(bool modif)
        {
            txbDVDComNbCommande.Enabled = false;
            dtpDVDComDateCommande.Enabled = !modif;
            txbDVDComMontant.Enabled = !modif;
            txbDVDComNbExemplaires.Enabled = !modif;
            txbDVDComNumDVD.Enabled = false;
            cbxDVDComEtapeSuivi.Enabled = !modif;
            btnDVDComAjouter.Enabled = modif;
            btnDVDComModifier.Enabled = modif;
            btnDVDComSupprimer.Enabled = modif;
            btnDVDComAnnuler.Enabled = !modif;
            btnDVDComValider.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// Événement de sélection d'une commande dans la liste
        /// </summary>
        private void DgvComDVDUn_SelectionChanged(object sender, EventArgs e)
        {
            bloquerGesEtape = true;

            if (dgvComDVDUn.CurrentCell != null)
            {
                try
                {
                    CommandeDocument comDvd = (CommandeDocument)bdgComUnDVD.List[bdgComUnDVD.Position];
                    if (comDvd.IdLivreDvd == "")
                    {
                        Dvd dvd = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
                        AfficheComDVDUn(comDvd, dvd);
                    }
                    else
                    {
                        AfficheComDVDUn(comDvd, null);
                    }
                }
                catch
                {
                    VideComDvdInfos();
                }
            }
            else
            {
                VideComDVDUnInfos();
            }

            bloquerGesEtape = false;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des détails de commande
        /// </summary>
        private void VideComDVDUnInfos()
        {
            if (!ajouterBool)
            {
                txbDVDComNbCommande.Text = "";
            }
            dtpDVDComDateCommande.MaxDate = DateTime.Today;
            txbDVDComMontant.Text = "";
            txbDVDComNbExemplaires.Text = "";
            cbxDVDComEtapeSuivi.SelectedIndex = -1;
        }

        /// <summary>
        /// Crée ou met à jour une commande selon le contexte
        /// </summary>
        private void AjoutUpCommandeDVD(CommandeDocument commandeDocument)
        {
            if (ajouterBool)
            {
                bool commandeCreer = controller.CreerCommandeDocument(commandeDocument);
                MessageBox.Show("Ajout de la nouvelle commande");

                if (!commandeCreer)
                {
                    MessageBox.Show("Erreur lors de la création de la commande");
                }
            }
            else if (!ajouterBool)
            {
                try
                {
                    bloquerGesEtape = true;
                    controller.UpdateCommandeDocument(commandeDocument);
                    MessageBox.Show("Modification de la commande effectuée");
                }
                catch
                {
                    MessageBox.Show("Erreur lors de la modification de la commande");
                }
            }
        }

        /// <summary>
        /// Événement de clic sur le bouton d'ajout de commande
        /// </summary>
        private void btnDVDComAjouter_Click(object sender, EventArgs e)
        {
            msgBoxSuivi = false;
            ModifComDVD(false);
            ajouterBool = true;
            VideComDVDUnInfos();
            bloquerGesEtape = true;
            cbxDVDComEtapeSuivi.SelectedIndexChanged -= cbxDVDComEtapeSuivi_SelectedIndexChanged;
            try
            {
                cbxDVDComEtapeSuivi.SelectedIndex = 0;
            }
            catch
            {
                return;
            }
            cbxDVDComEtapeSuivi.SelectedIndexChanged += cbxDVDComEtapeSuivi_SelectedIndexChanged;
            cbxDVDComEtapeSuivi.Enabled = false;
            dtpDVDComDateCommande.Enabled = false;
            msgBoxSuivi = true;

            try
            {
                string maxId = controller.GetMaxIdCommande();

                if (string.IsNullOrEmpty(maxId))
                {
                    MessageBox.Show("Erreur : aucun ID existant trouvé");
                    maxId = "0";
                }

                string newId = IdMaxPlusUn(maxId);

                if (!string.IsNullOrEmpty(newId))
                {
                    txbDVDComNbCommande.Text = newId;
                }
                else
                {
                    MessageBox.Show("Erreur lors de la génération du nouvel ID");
                    txbDVDComNbCommande.Text = "00001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur critique : {ex.Message}");
                txbDVDComNbCommande.Text = "00001";
            }
            bloquerGesEtape = false;
        }

        /// <summary>
        /// Événement de clic sur le bouton de modification de commande
        /// </summary>
        private void btnDVDComModifier_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            modifBool = true;
            ModifComDVD(false);
        }

        /// <summary>
        /// Événement de clic sur le bouton de validation
        /// Gère la validation selon le contexte (ajout/modification/suppression)
        /// </summary>
        private void btnDVDComValider_Click(object sender, EventArgs e)
        {
            bloquerGesEtape = true;

            if (supprComDVD)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer cette commande ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CommandeDocument comListeDelete = (CommandeDocument)bdgComUnDVD.List[bdgComUnDVD.Position];
                    if (dgvComDVDUn.CurrentCell != null && txbDVDComNbCommande.Text != "")
                    {
                        if (comListeDelete.IdSuivi <= 2)
                        {
                            bool resultDelete = controller.DeleteCommandeDoc(comListeDelete);
                            if (resultDelete)
                            {
                                MessageBox.Show("Commande supprimée");
                                VideComDVDUnInfos();
                                List<CommandeDocument> comListMaj = controller.GetCommandeDocument(comListeDelete.IdLivreDvd);
                                RemplirComDVDDetails(comListMaj);
                                supprComDVD = false;
                            }
                            else
                            {
                                MessageBox.Show("Erreur lors de la suppression de la commande");
                            }
                            supprComDVD = false;
                        }
                        else
                        {
                            MessageBox.Show("Vous ne pouvez pas supprimer une commande livrée ou réglée !");
                            supprComDVD = false;
                        }
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir valider cette action ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string id = txbDVDComNbCommande.Text;
                    DateTime dateCommande = dtpDVDComDateCommande.Value;
                    float montant = float.TryParse(txbDVDComMontant.Text, out _) ? float.Parse(txbDVDComMontant.Text) : -1;
                    int nbExemplaire = int.TryParse(txbDVDComNbExemplaires.Text, out _) ? int.Parse(txbDVDComNbExemplaires.Text) : -1;
                    string idLivreDvd = txbDVDComNumDVD.Text;
                    int idSuivi = 0;
                    string etapeSuivi = "";
                    Suivi suivi = (Suivi)cbxDVDComEtapeSuivi.SelectedItem;
                    VerifValueDVDCom();
                    VideComDVDUnInfos();
                    if (suivi != null)
                    {
                        idSuivi = suivi.IdSuivi;
                        etapeSuivi = suivi.EtapeSuivi;
                    }
                    if (montant != -1 && nbExemplaire != -1 && etapeSuivi != "")
                    {
                        CommandeDocument commandeDVD = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etapeSuivi);
                        AjoutUpCommandeDVD(commandeDVD);
                        if (ajouterBool)
                        {
                            ajouterBool = false;
                        }
                        List<CommandeDocument> comListMaj = controller.GetCommandeDocument(idLivreDvd);
                        RemplirComDVDDetails(comListMaj);
                    }
                }
            }
            ModifComDVD(true);
        }

        /// <summary>
        /// Vérifie la validité des valeurs saisies pour une commande
        /// </summary>
        private void VerifValueDVDCom()
        {
            float montant = -1;
            int nbExemplaire = -1;

            try
            {
                montant = float.Parse(txbDVDComMontant.Text);
                nbExemplaire = int.Parse(txbDVDComNbExemplaires.Text);
            }
            catch
            {
                if (montant == -1)
                {
                    MessageBox.Show("Le montant doit comporter une virgule !");
                }

                if (nbExemplaire == -1)
                {
                    MessageBox.Show("Le nombre d'exemplaires doit être un nombre entier !");
                }
            }
            Suivi suivi = (Suivi)cbxDVDComEtapeSuivi.SelectedItem;
            if (suivi == null)
            {
                MessageBox.Show("Vous devez sélectionner une étape");
            }
        }

        /// <summary>
        /// Événement de clic sur le bouton d'annulation
        /// </summary>
        private void btnDVDComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            ModifComDVD(true);
            bloquerGesEtape = true;
            Dvd dvdRecupId = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
            List<CommandeDocument> comListMaj = controller.GetCommandeDocument(dvdRecupId.Id);
            RemplirComDVDDetails(comListMaj);
        }

        /// <summary>
        /// Événement de clic sur le bouton de suppression de commande
        /// </summary>
        private void btnDVDComSupprimer_Click(object sender, EventArgs e)
        {
            supprComDVD = true;
            ModifComDVD(true);
            btnDVDComAjouter.Enabled = false;
            btnDVDComModifier.Enabled = false;
            btnDVDComAnnuler.Enabled = true;
            btnDVDComValider.Enabled = true;
        }

        /// <summary>
        /// Événement de changement de sélection dans la liste des étapes de suivi
        /// </summary>
        private void cbxDVDComEtapeSuivi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bloquerGesEtape)
            {
                return;
            }

            int nvEtape = cbxDVDComEtapeSuivi.SelectedIndex;
            GestionCbxSuivi(nvEtape);
        }

        #endregion

        #region Onglet CommandeRevues
        private readonly BindingSource bdgComRevuesListe = new BindingSource();
        private readonly BindingSource bdgComUneRevue = new BindingSource();
        private List<Revue> lesComRevues = new List<Revue>();

        /// <summary>
        /// Événement déclenché lors de l'entrée dans l'onglet Commande de Revues
        /// Initialise les listes et combos nécessaires à la gestion des commandes
        /// </summary>
        private void tabComRevues_Enter(object sender, EventArgs e)
        {
            lesComRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComRevuesRayons);
            RemplirComRevuesListeComplete();
            ModifComRevue(true);
        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des revues fournie
        /// Configure les colonnes visibles et leur ordre d'affichage
        /// </summary>
        private void RemplirComRevuesListe(List<Revue> Comrevues)
        {
            bdgComRevuesListe.DataSource = Comrevues;
            dgvComRevuesListe.DataSource = bdgComRevuesListe;
            dgvComRevuesListe.Columns["idRayon"].Visible = false;
            dgvComRevuesListe.Columns["idGenre"].Visible = false;
            dgvComRevuesListe.Columns["idPublic"].Visible = false;
            dgvComRevuesListe.Columns["image"].Visible = false;
            dgvComRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvComRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche une revue par son numéro et l'affiche
        /// Affiche un message si le numéro n'est pas trouvé
        /// </summary>
        private void btnComRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComRevuesNumRecherche.Text.Equals(""))
            {
                txbComRevuesTitreRecherche.Text = "";
                cbxComRevuesGenres.SelectedIndex = -1;
                cbxComRevuesRayons.SelectedIndex = -1;
                cbxComRevuesPublics.SelectedIndex = -1;
                Revue revue = lesComRevues.Find(x => x.Id.Equals(txbComRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirComRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComRevuesListeComplete();
                }
            }
            else
            {
                RemplirComRevuesListeComplete();
            }
        }

        /// <summary>
        /// Filtre dynamique des revues par titre au fur et à mesure de la saisie
        /// </summary>
        private void txbComRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComRevuesTitreRecherche.Text.Equals(""))
            {
                cbxComRevuesGenres.SelectedIndex = -1;
                cbxComRevuesRayons.SelectedIndex = -1;
                cbxComRevuesPublics.SelectedIndex = -1;
                txbComRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesComRevues.FindAll(x => x.Titre.ToLower().Contains(txbComRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                if (cbxComRevuesGenres.SelectedIndex < 0 && cbxComRevuesPublics.SelectedIndex < 0 && cbxComRevuesRayons.SelectedIndex < 0
                    && txbComRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirComRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affiche les informations détaillées de la revue sélectionnée
        /// </summary>
        private void AfficheComRevuesInfos(Revue revue)
        {
            txbComRevuesNumero.Text = revue.Id;
            txbComRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbComRevuesPeriodicite.Text = revue.Periodicite;
            txbComRevuesGenre.Text = revue.Genre;
            txbComRevuesPublic.Text = revue.Public;
            txbComRevuesRayon.Text = revue.Rayon;
            txbComRevuesTitre.Text = revue.Titre;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des informations de la revue
        /// </summary>
        private void VideComRevuesInfos()
        {
            txbComRevuesNumero.Text = "";
            txbComRevuesDateMiseADispo.Text = "";
            txbComRevuesPeriodicite.Text = "";
            txbComRevuesGenre.Text = "";
            txbComRevuesPublic.Text = "";
            txbComRevuesRayon.Text = "";
            txbComRevuesTitre.Text = "";
        }

        /// <summary>
        /// Filtre les revues par genre sélectionné
        /// </summary>
        private void cbxComRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComRevuesGenres.SelectedIndex >= 0)
            {
                txbComRevuesTitreRecherche.Text = "";
                txbComRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxComRevuesGenres.SelectedItem;
                List<Revue> revues = lesComRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComRevuesListe(revues);
                cbxComRevuesRayons.SelectedIndex = -1;
                cbxComRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les revues par public sélectionné
        /// </summary>
        private void cbxComRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComRevuesPublics.SelectedIndex >= 0)
            {
                txbComRevuesTitreRecherche.Text = "";
                txbComRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxComRevuesPublics.SelectedItem;
                List<Revue> revues = lesComRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComRevuesListe(revues);
                cbxComRevuesRayons.SelectedIndex = -1;
                cbxComRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre les revues par rayon sélectionné
        /// </summary>
        private void cbxComRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComRevuesRayons.SelectedIndex >= 0)
            {
                txbComRevuesTitreRecherche.Text = "";
                txbComRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxComRevuesRayons.SelectedItem;
                List<Revue> revues = lesComRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComRevuesListe(revues);
                cbxComRevuesGenres.SelectedIndex = -1;
                cbxComRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Événement de sélection d'une revue dans la liste
        /// Affiche les informations de la revue et ses abonnements associés
        /// </summary>
        private void dgvComRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgComRevuesListe.List[bdgComRevuesListe.Position];
                    txbRevueComNumRevue.Text = revue.Id;
                    AfficheComRevuesInfos(revue);

                    List<Abonnement> comRevue = controller.GetCommandeRevue(revue.Id);
                    RemplirComRevueDetails(comRevue);

                    if (ajouterBool)
                    {
                        ModifComRevue(true);
                        ajouterBool = false;
                        if (comRevue.Count == 0)
                        {
                            txbComRevuesNumero.Text = "";
                        }
                    }

                    if (supprComRevue)
                    {
                        supprComRevue = false;
                        List<Abonnement> comAboSuppr = controller.GetCommandeRevue(revue.Id);
                        RemplirComRevueDetails(comAboSuppr);
                        ModifComRevue(true);
                    }

                    if (modifBool)
                    {
                        modifBool = false;
                        List<Abonnement> comAboSuppr = controller.GetCommandeRevue(revue.Id);
                        RemplirComRevueDetails(comAboSuppr);
                        ModifComRevue(true);
                    }
                }
                catch
                {
                    VideComRevuesZones();
                }
            }
            else
            {
                VideComRevuesInfos();
            }
        }

        /// <summary>
        /// Annule le filtre par public et réaffiche la liste complète
        /// </summary>
        private void btnComRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComRevuesListeComplete();
        }

        /// <summary>
        /// Annule le filtre par rayon et réaffiche la liste complète
        /// </summary>
        private void btnComRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComRevuesListeComplete();
        }

        /// <summary>
        /// Annule le filtre par genre et réaffiche la liste complète
        /// </summary>
        private void btnComRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComRevuesListeComplete();
        }

        /// <summary>
        /// Affiche la liste complète des revues et réinitialise les filtres
        /// </summary>
        private void RemplirComRevuesListeComplete()
        {
            RemplirComRevuesListe(lesComRevues);
            VideComRevuesZones();
        }

        /// <summary>
        /// Réinitialise les zones de recherche et de filtre
        /// </summary>
        private void VideComRevuesZones()
        {
            cbxComRevuesGenres.SelectedIndex = -1;
            cbxComRevuesRayons.SelectedIndex = -1;
            cbxComRevuesPublics.SelectedIndex = -1;
            txbComRevuesNumRecherche.Text = "";
            txbComRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Trie les revues selon la colonne cliquée
        /// </summary>
        private void dgvComRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComRevuesZones();
            string titreColonne = dgvComRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesComRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesComRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesComRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComRevuesListe(sortedList);
        }

        /// <summary>
        /// Remplit la liste des abonnements pour la revue sélectionnée
        /// </summary>
        private void RemplirComRevueDetails(List<Abonnement> Comrevue)
        {
            bdgComUneRevue.DataSource = Comrevue;
            dgvComRevueUne.DataSource = bdgComUneRevue;
            dgvComRevueUne.Columns["idRevue"].Visible = false;
            dgvComRevueUne.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComRevueUne.Columns["dateCommande"].DisplayIndex = 0;
        }

        /// <summary>
        /// Affiche les détails de l'abonnement sélectionné
        /// </summary>
        private void AfficheComRevueUn(Abonnement Comrevue, Revue revue)
        {
            txbRevueComNbAbonnement.Text = Comrevue.Id;
            dtpRevueDateCommande.Value = Comrevue.DateCommande;
            txbRevueComMontant.Text = Comrevue.Montant.ToString();
            dtpRevueFinAbonnement.Value = Comrevue.DateFinAbonnement;
            if (Comrevue.IdRevue == "")
            {
                txbRevueComNumRevue.Text = revue.Id;
            }
            else
            {
                txbRevueComNumRevue.Text = Comrevue.IdRevue;
            }
        }

        /// <summary>
        /// Active/désactive les contrôles pour l'édition d'un abonnement
        /// </summary>
        private void ModifComRevue(bool modif)
        {
            txbRevueComNbAbonnement.Enabled = false;
            dtpRevueDateCommande.Enabled = !modif;
            txbRevueComMontant.Enabled = !modif;
            dtpRevueFinAbonnement.Enabled = !modif;
            txbRevueComNumRevue.Enabled = false;
            btnRevueComAjouter.Enabled = modif;
            btnRevueComModifier.Enabled = modif;
            btnRevueComSupprimer.Enabled = modif;
            btnRevueComAnnuler.Enabled = !modif;
            btnRevueComValider.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// Événement de sélection d'un abonnement dans la liste
        /// </summary>
        private void DgvComRevueUn_SelectionChanged(object sender, EventArgs e)
        {
            bloquerGesEtape = true;

            if (dgvComRevueUne.CurrentCell != null)
            {
                try
                {
                    Abonnement comRevue = (Abonnement)bdgComUneRevue.List[bdgComUneRevue.Position];
                    if (comRevue.IdRevue == "")
                    {
                        Revue revue = (Revue)bdgComRevuesListe.List[bdgComRevuesListe.Position];
                        AfficheComRevueUn(comRevue, revue);
                    }
                    else
                    {
                        AfficheComRevueUn(comRevue, null);
                    }
                }
                catch
                {
                    VideComRevueUnInfos();
                }
            }
            else
            {
                VideComRevueUnInfos();
            }

            bloquerGesEtape = false;
        }

        /// <summary>
        /// Réinitialise les zones d'affichage des détails d'abonnement
        /// </summary>
        private void VideComRevueUnInfos()
        {
            if (!ajouterBool)
            {
                txbRevueComNbAbonnement.Text = "";
            }
            dtpRevueDateCommande.MaxDate = DateTime.Today;
            txbRevueComMontant.Text = "";
            dtpRevueFinAbonnement.Value = DateTime.Today;
        }

        /// <summary>
        /// Événement de clic sur le bouton d'ajout d'abonnement
        /// </summary>
        private void btnRevueComAjouter_Click(object sender, EventArgs e)
        {
            ModifComRevue(false);
            dtpRevueDateCommande.Enabled = false;
            ajouterBool = true;
            VideComRevueUnInfos();

            try
            {
                string maxId = controller.GetMaxIdCommande();

                if (string.IsNullOrEmpty(maxId))
                {
                    MessageBox.Show("Erreur : aucun ID existant trouvé");
                    maxId = "0";
                }

                string newId = IdMaxPlusUn(maxId);

                if (!string.IsNullOrEmpty(newId))
                {
                    txbRevueComNbAbonnement.Text = newId;
                }
                else
                {
                    MessageBox.Show("Erreur lors de la génération du nouvel ID");
                    txbRevueComNbAbonnement.Text = "00001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur critique : {ex.Message}");
                txbRevueComNbAbonnement.Text = "00001";
            }
        }

        /// <summary>
        /// Événement de clic sur le bouton de modification d'abonnement
        /// </summary>
        private void btnRevueComModifier_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            modifBool = true;
            ModifComRevue(false);
            dtpRevueDateCommande.Enabled = false;
        }

        /// <summary>
        /// Événement de clic sur le bouton de validation
        /// Gère la validation selon le contexte (ajout/modification/suppression)
        /// </summary>
        private void btnRevueComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir valider cette action ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbRevueComNbAbonnement.Text;
                DateTime dateCommande = dtpRevueDateCommande.Value;
                float montant = float.TryParse(txbRevueComMontant.Text, out _) ? float.Parse(txbRevueComMontant.Text) : -1;
                DateTime dateFinAbonnement = dtpRevueFinAbonnement.Value;
                string idRevue = txbRevueComNumRevue.Text;

                if (montant != -1 && !string.IsNullOrWhiteSpace(idRevue))
                {
                    Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);

                    if (ajouterBool)
                    {
                        bool commandeCreer = controller.CreerAbonnement(abonnement);
                        MessageBox.Show("Ajout de la nouvelle commande");

                        if (!commandeCreer)
                        {
                            MessageBox.Show("Erreur lors de la création de la commande");
                        }
                        ajouterBool = false;
                    }
                    else
                    {
                        try
                        {
                            controller.UpdateAbonnement(abonnement);
                            MessageBox.Show("Modification de la commande effectuée");
                        }
                        catch
                        {
                            MessageBox.Show("Erreur lors de la modification de la commande");
                        }
                    }

                    List<Abonnement> comListMaj = controller.GetCommandeRevue(idRevue);
                    RemplirComRevueDetails(comListMaj);
                }
            }

            ModifComRevue(true);
        }

        /// <summary>
        /// Événement de clic sur le bouton d'annulation
        /// </summary>
        private void btnRevueComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            ModifComRevue(true);

            Revue revueRecup = (Revue)bdgComRevuesListe.List[bdgComRevuesListe.Position];
            List<Abonnement> comListMaj = controller.GetCommandeRevue(revueRecup.Id);
            RemplirComRevueDetails(comListMaj);
        }

        /// <summary>
        /// Événement de clic sur le bouton de suppression d'abonnement
        /// Vérifie qu'il n'y a pas d'exemplaires associés avant suppression
        /// </summary>
        private void btnRevueComSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvComRevueUne.CurrentCell != null)
            {
                Abonnement AboRecup = (Abonnement)bdgComUneRevue.List[bdgComUneRevue.Position];
                DateTime dateCommande = AboRecup.DateCommande;
                DateTime DateFinAbonnement = AboRecup.DateFinAbonnement;
                List<Exemplaire> recupDateParution = controller.GetExemplairesRevue(AboRecup.IdRevue);
                bool exemplaireDansAbonnement = recupDateParution.Any(exp => ParutionDansAbonnement(dateCommande, DateFinAbonnement, exp.DateAchat));

                if (!exemplaireDansAbonnement)
                {
                    if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer cette commande ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Abonnement abonnement = (Abonnement)bdgComUneRevue.List[bdgComUneRevue.Position];
                        bool resultDelete = controller.DeleteAbonnement(abonnement);

                        if (resultDelete)
                        {
                            MessageBox.Show("Commande supprimée");
                            VideComRevueUnInfos();
                            List<Abonnement> comListMaj = controller.GetCommandeRevue(abonnement.IdRevue);
                            RemplirComRevueDetails(comListMaj);
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de la commande");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vous ne pouvez pas supprimer cette commande de revue car elle contient des exemplaires !");
                }
            }
        }

        /// <summary>
        /// Vérifie si un exemplaire est compris dans la période d'abonnement
        /// </summary>
        private bool ParutionDansAbonnement(DateTime dateCommande, DateTime DateFinAbonnement, DateTime dateParution)
        {
            return dateParution >= dateCommande && dateParution <= DateFinAbonnement;
        }

        /// <summary>
        /// Affiche les abonnements arrivant à expiration dans moins de 30 jours
        /// </summary>
        public void VerifTempsAbo()
        {
            if (serviceUtili == "administrateur" || serviceUtili == "administratif")
            {
                bool moins30j = false;
                List<Revue> getRevues = controller.GetAllRevues();
                string msg30 = "Les abonnement qui expire dans moins de 30 jours sont : \n \n";

                foreach (Revue revue in getRevues)
                {
                    List<Abonnement> getAboRevue = controller.GetCommandeRevue(revue.Id);
                    foreach (Abonnement abo in getAboRevue)
                    {
                        DateTime dateFinAbonnement = abo.DateFinAbonnement;

                        TimeSpan tmpsRestant = dateFinAbonnement - DateTime.Today;

                        if (tmpsRestant.Days <= 30 && tmpsRestant.Days >= 0)
                        {
                            msg30 += "titre : " + revue.Titre + " date de fin d'abonnement : " + abo.DateFinAbonnement + "\n";
                            moins30j = true;
                        }
                    }
                }

                if (moins30j)
                {
                    MessageBox.Show(msg30);
                }
            }
        }

        #endregion
    }
}
