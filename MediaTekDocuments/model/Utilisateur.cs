using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        public int Id;
        public string Login;
        public string Password;
        public int IdService;
        public string Service;

        public Utilisateur(int id, string login, string password, int idService, string service)
        {
            this.Id = id;
            this.Login = login;
            this.Password = password;
            this.IdService = idService;
            this.Service = service;
        }
    }
}
