using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace StagePFE.Areas.Administrateur.Controllers
{
    public class ListeEmployeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public ListeEmployeController()
        {
        }

        public ListeEmployeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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

       
            // GET: Administrateur/Employe
            public ActionResult ListeEmploye()
            {
                CongesViewModels cv = new CongesViewModels();
                CongesEntities d = new CongesEntities();
                var employes = from employe in d.Employe
                               
                               select employe
                          ;
                cv.Employe = employes.ToList();
                return View(cv);
            }

            public ActionResult Liste()
            {
                CongesViewModels cv = new CongesViewModels();
                CongesEntities d = new CongesEntities();
                var employes = from employe in d.Employe

                               select employe
                          ;
                cv.Employe = employes.ToList();
                return View(cv);
            }

        public ActionResult NouveauUtilisateur()
        {
            return View();
        }

        public ActionResult Employes()
        {
            
            var employe = db.Employe.Include(e => e.AspNetUsers);
            return View(employe.ToList());
        }

        public ActionResult NouvelleEquipe()
        {
            var equipe = db.Equipes.Include(e => e.Employe).Include(p => p.Departements);
            return View(equipe.ToList());

        }

        public ActionResult DemandesEnAttente()
        {
            IEnumerable<Demandes> demande = db.Demandes
                 .Where(u => u.ApprobationChef == "Accepté").ToList();
            CongesViewModels cv = new CongesViewModels();
            cv.Demande = demande;



            return View(cv);
        }

        public ActionResult CreerEquipe()
        {
            
                ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement");
                return View();
            }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreerEquipe([Bind(Include = "Id,IdChef,IdDep")] Equipes equipes)
        {
            if (ModelState.IsValid)
            {
                db.Equipes.Add(equipes);
                db.SaveChanges();
                return RedirectToAction("NouvelleEquipe", "ListeEmploye");
            }

            ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement", equipes.IdDep);
            return View(equipes);
        }

        public ActionResult SupprimerEquipe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Equipes equipe = db.Equipes.Find(id);
            if (equipe == null)
            {
                return HttpNotFound();
            }

            db.Equipes.Remove(equipe);
            db.SaveChanges();
            return RedirectToAction("NouvelleEquipe", "ListeEmploye");
        }

    }

   
    }
