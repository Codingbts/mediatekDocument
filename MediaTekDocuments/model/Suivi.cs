using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi
    /// </summary>
    public class Suivi
    {
        public int IdSuivi;
        public string EtapeSuivi;

        public Suivi (int idSuivi, string etapeSuivi)
        {
            this.IdSuivi = idSuivi;
            this.EtapeSuivi = etapeSuivi;
        }

        /// <summary>
        /// Récupération de etapeSuivi pour l'affichage dans les combos
        /// </summary>
        /// <returns>Libelle</returns>
        public override string ToString()
        {
            return this.EtapeSuivi;
        }
    }
}
