using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StagePFE.Areas.ChefProjet.Models
{
    public class DashboardChefViewModel
    {

        public virtual IEnumerable<Employe> Employes { get; set; }
        public virtual IEnumerable<Equipes> Equipe { get; set; }
        public virtual IEnumerable<Demandes> Demande { get; set; }
        public virtual IEnumerable<Demandes> Demandes { get; set; }

    }
}