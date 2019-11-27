using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StagePFE.Areas.Administrateur.Models
{
    public class DashboardViewModel
    {
        public virtual IEnumerable<Employe>  Employes { get; set; }
        public virtual IEnumerable<Equipes>  Equipe { get; set; }
        public virtual IEnumerable<Demandes> Demande { get; set; }
        public virtual IEnumerable<Employe> Homme { get; set; }
        public virtual IEnumerable<Employe> Femme { get; set; }

    }
}