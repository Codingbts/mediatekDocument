using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Service
    /// </summary>
    public class Service
    {
        public int Id;
        public string Libelle;

        public Service(int id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
