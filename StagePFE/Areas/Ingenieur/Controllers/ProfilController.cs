    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using StagePFE.Models;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.AspNet.Identity;
using StagePFE.Areas.Ingenieur.Models;

namespace StagePFE.Areas.Ingenieur.Controllers
{
        public class ProfilController : Controller
        {

            private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;
            private ApplicationDbContext _context;
            private CongesEntities db = new CongesEntities();

            public ProfilController()
            {
            }

            public ProfilController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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

            //GET: profile
            public ActionResult ProfilIngenieur()
            {
            ProfilViewModel p = new ProfilViewModel();
                var user = UserManager.FindById(User.Identity.GetUserId());
            IEnumerable < Employe > employe = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .ToList();
            var id = db.Employe
                   .Where(u => u.IdUser == user.Id)
                   .FirstOrDefault().Id;
            var idequipe = db.Employe
                   .Where(u => u.IdUser == user.Id)
                   .FirstOrDefault().IdEquipe;
            IEnumerable<Employe> equipe = db.Employe
                  .Where(u=>u.IdEquipe==idequipe)
                  .ToList();
            IEnumerable<Compte> compte = db.Compte
                  .Where(u => u.IdEmploye ==id)
                  .ToList();
            p.Employes=employe;
            p.equipes = equipe;
            p.Comptes = compte;
            return View(p);

                //Employe employe = db.Employe.Find(id);
                //if (employe == null)
                //{
                //    return HttpNotFound();
                //}
                //return View(employe);
            }



        public ActionResult Modification(String cin,String tel, String adresse)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            Employe employe = db.Employe
                 .Where(u => u.IdUser == user.Id)
                 .FirstOrDefault();

           
            employe.Adresse = adresse;
            employe.Telephone = tel;
            employe.Cin = cin;
            db.SaveChanges();
            return RedirectToAction("ProfilIngenieur", "Profil", new { Areas = "Ingenieur" });
        }

        // GET: Employes
        public ActionResult Index()
            {
                var employe = db.Employe.Include(e => e.AspNetUsers);
                return View(employe.ToList());
            }

            // GET: Employes/Details/5
            public ActionResult Details(int? id)
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
                return View(employe);
            }

            // GET: Employes/Create
            public ActionResult Create()
            {
                ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email");
                return View();
            }

            // POST: Employes/Create
            // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
            // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "Id,Nom,Prenom,Poste,DateRecrutement,DateNaissance,Sexe,Telephone,Adresse,Cin,IdUser,IdEquipe")] Employe employe)
            {
                if (ModelState.IsValid)
                {
                    db.Employe.Add(employe);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", employe.IdUser);
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
                    return RedirectToAction("Index");
                }
                ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", employe.IdUser);
                return View(employe);
            }

            // GET: Employes/Delete/5
            public ActionResult Delete(int? id)
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
                return View(employe);
            }

            // POST: Employes/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Employe employe = db.Employe.Find(id);
                db.Employe.Remove(employe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
