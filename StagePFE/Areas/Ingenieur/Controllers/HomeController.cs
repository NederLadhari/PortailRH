using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Areas.Ingenieur.Models;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.Ingenieur.Controllers
{
    public class HomeController : Controller
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

        public ActionResult Index()
        {
            return View();
        }

       public ActionResult Accueil()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            int? idequipe = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().IdEquipe;

            AccueilViewModel cv = new AccueilViewModel();

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(x => x.Etat == "En cours de traitement" || x.Etat=="En cours de validation")
                 
                  .Where(p => p.EmployeID == idEmploye)
                  .ToList();
            IEnumerable<Equipes> equipe = db.Equipes
                   .Where(x => x.Id==idequipe)
                  .ToList();
            IEnumerable<Employe> employe = db.Employe
                   .Where(x => x.Id==idEmploye)
                  .ToList();


            cv.Demande = demande;
            cv.Equipe = equipe;
            cv.Employes = employe;


            return View(cv);
        }
    }
}