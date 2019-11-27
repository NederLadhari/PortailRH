using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StagePFE.Areas.Ingenieur.Models
{
    public class ProfilViewModel
    {
        public virtual IEnumerable<Employe> Employes { get; set; }
        public virtual IEnumerable<Compte> Comptes { get; set; }
        public virtual IEnumerable<Employe> equipes { get; set; }
    }
}