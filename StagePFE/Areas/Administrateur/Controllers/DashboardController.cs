using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Areas.Administrateur.Models;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.Administrateur.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Administrateur/Dashboard
        public ActionResult DashboardAdmin()
        {
            DashboardViewModel cv = new DashboardViewModel();

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(x => x.Etat == "En cours")
                  .ToList();
            IEnumerable<Equipes> equipe = db.Equipes
                  .ToList();
            IEnumerable<Employe> employe = db.Employe
                  .ToList();

            IEnumerable<Employe> homme = db.Employe
                 .Where(x => x.Sexe=="Homme")
                .ToList();

            IEnumerable<Employe> femme = db.Employe
                 .Where(x => x.Sexe == "Femme")
                .ToList();

            cv.Femme = femme;
            cv.Homme = homme;
            cv.Demande = demande;
            cv.Equipe = equipe;
            cv.Employes = employe;


            return View(cv);
        }

        
    }
}