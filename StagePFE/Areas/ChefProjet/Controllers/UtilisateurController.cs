using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.ChefProjet.Controllers
{
    public class UtilisateurController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public UtilisateurController()
        {
        }

        public UtilisateurController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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

        // GET: ChefProjet/Utilisateur
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Calendrier()
        {
            CongesViewModels cv = new CongesViewModels();

            var user = UserManager.FindById(User.Identity.GetUserId());
            int? idequipe = db.Employe
                  .Where(u => u.IdUser == user.Id)
                   .FirstOrDefault().IdEquipe;

            var demandes = from demande in db.Demandes
                           join employe in db.Employe on demande.EmployeID equals employe.Id
                           join equipe in db.Equipes on employe.IdEquipe equals equipe.Id
                           where equipe.Id == idequipe.Value
                           where demande.Etat == "Accepté"
                           select demande
                           ;

            cv.Demande = demandes.ToList();



            return View(cv);
        }

        public ActionResult ProfilChef()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int id = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            Employe employe = db.Employe.Find(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            return View(employe);
        }

        // GET: Employes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = db.Employe.Find(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", employe.IdUser);
            return View(employe);
        }

        // POST: Employes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nom,Prenom,Poste,DateRecrutement,DateNaissance,Sexe,Telephone,Adresse,Cin,IdUser,IdEquipe")] Employe employe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilChef");
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", employe.IdUser);
            return View(employe);
        }


        public ActionResult getDate()
        {
            CongesViewModels cv = new CongesViewModels();

            var demande = db.Demandes.Where(x => x.Etat=="Acceptée").ToList();
            return Json(new { data = demande }, JsonRequestBehavior.AllowGet);
        }
       




    }
}