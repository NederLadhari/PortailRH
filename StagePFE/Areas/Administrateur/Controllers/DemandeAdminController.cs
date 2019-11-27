using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.Administrateur.Controllers
{
    public class DemandeAdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();
        public DemandeAdminController()
        {
        }

        public DemandeAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            this._context = _context;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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
        // GET: Administrateur/DemandeAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DemandeEnAttente()
        {
            CongesViewModels cv = new CongesViewModels();

            var demandess = from demande in db.Demandes
                            join employe in db.Employe on demande.EmployeID equals employe.Id

                            where demande.ApprobationChef == "Accepté"
                            where demande.Etat=="En cours"
                            select demande
                           ;
            cv.Demande = demandess.ToList();
            return View(cv);
        }


        public ActionResult AccepterDemande(int? id, int? em, int nbr)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Demandes demandes = db.Demandes.Find(id);
            if (demandes == null)
            {
                return HttpNotFound();
            }

            demandes.ApprobationGerant = "Acceptée";
            demandes.Etat = "Traitée";
            db.SaveChanges();

            Employe employe = db.Employe.Find(em);
            if (employe == null)
            {
                return HttpNotFound();
            }
            Compte compte = db.Compte
                  .Where(u => u.IdEmploye == employe.Id)
                  .FirstOrDefault();
            var solde = db.Compte
                .Where(u => u.IdEmploye == employe.Id)
                .FirstOrDefault().Solde;

            compte.Solde = solde - nbr;

            db.SaveChanges();

            return RedirectToAction("DemandeEnAttente");


            //Historique



            //return RedirectToAction("EnAttente", "Demandes");
        }

        public ActionResult RejetterDemande(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Demandes demandes = db.Demandes.Find(id);
            if (demandes == null)
            {
                return HttpNotFound();
            }

            demandes.ApprobationGerant = "Refusé";
            demandes.Etat = "Refusé";
            db.SaveChanges();
            return View();
        }
    }
}