using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Areas.ChefProjet.Models;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.ChefProjet.Controllers
{
    public class DashboardChefController : Controller
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


        // GET: ChefProjet/DashboardChef
        public ActionResult DashboardChefProjet()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            int? idequipe = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().IdEquipe;

            DashboardChefViewModel cv = new DashboardChefViewModel();

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(x => x.Etat == "En cours de Traitement"||x.Etat== "En cours de validation")
                  .Where(p => p.EmployeID == idEmploye)
                  .ToList();
            IEnumerable<Equipes> equipe = db.Equipes
                   .Where(x => x.Id == idequipe)
                  .ToList();
            IEnumerable<Employe> employe = db.Employe
                   .Where(x => x.Id == idEmploye)
                  .ToList();

            var demandes = from demandee in db.Demandes
                           join employee in db.Employe on demandee.EmployeID equals employee.Id
                           join equipee in db.Equipes on employee.IdEquipe equals equipee.Id
                           where equipee.Id == idequipe.Value
                           where demandee.Etat == "En cours de Traitement"
                           select demandee
                           ;
            cv.Demandes = demandes.ToList();


            cv.Demande = demande;
            cv.Equipe = equipe;
            cv.Employes = employe;


            return View(cv);
        }
    }
}