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
using System.Threading.Tasks;
using StagePFE.Areas.Administrateur.Models;

namespace StagePFE.Areas.Administrateur.Controllers
{
    public class ProfilAdminController : Controller
    {
        private CongesEntities db = new CongesEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        

        public ProfilAdminController()
        {
        }

        public ProfilAdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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
        
        
        public ActionResult ProfilAd()
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





        // GET: Employes
        
        public ActionResult Utilisateurs()
        {
            var employe = db.Employe.Include(e => e.AspNetUsers);
            return View(employe.ToList());
        }

        public ActionResult SupprimerUtilisateur(int? id)
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
            
            Compte compte = db.Compte.Where(x => x.IdEmploye == id).FirstOrDefault();
            db.Employe.Remove(employe);
            db.Compte.Remove(compte);
            db.SaveChanges();
            return RedirectToAction("Utilisateurs","ProfilAdmin", new { area = "Administrateur" });
           
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

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        public ActionResult MesEquipes()
        {
            //var equipe = db.Equipes.Include(e => e.Employe);
            //ViewBag.chef = new SelectList(db.Employe, "Nom", "Prenom");
            //ViewData["chef"] = db.Employe.ToList();

            DashboardViewModel cv = new DashboardViewModel();


            IEnumerable<Employe> empl = db.Employe

                   .ToList();


            var equipes = from equipee in db.Equipes
                           
                           join departement in db.Departements on equipee.IdDep equals departement.Id
                          
                           select equipee
                           ;
            var employes = from emp in db.Employe
                           select emp;


                          
            cv.Equipe = equipes.ToList();


            cv.Employes = empl;
            return View(cv);

        }
        public async Task<ActionResult> AffecterMembre(String email, int? id)
        
        {
            var user = await UserManager.FindByEmailAsync(email);
            var employe = db.Employe
                             .Where(u => u.IdUser == user.Id)
                             .FirstOrDefault();

            employe.IdEquipe = id;
            db.SaveChanges();

            return RedirectToAction("MesEquipes", "ProfilAdmin", new { area = "Administrateur" });
        }

        public async Task<ActionResult> ModifierChef(String email, int? id)

        {
            var user = await UserManager.FindByEmailAsync(email);
            var employe = db.Employe
                             .Where(u => u.IdUser == user.Id)
                             .FirstOrDefault();

            
            var equipe = db.Equipes
                             .Where(u => u.Id == id)
                             .FirstOrDefault();

            employe.IdEquipe = id;
            employe.Poste = "Chef de projet";
            equipe.IdChef = employe.Id;
           
            var r = UserManager.RemoveFromRole(user.Id, "Ingenieur");
            var result1 = UserManager.AddToRole(user.Id, "Chef de projet");
            db.SaveChanges();

            return RedirectToAction("MesEquipes", "ProfilAdmin", new { area = "Administrateur" });
        }


        // GET: Equipes/Create
        public ActionResult CreerEquipe()
        {
            ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement");
            return View();
        }

        // POST: Equipes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdChef,IdDep")] Equipes equipes)
        {
            if (ModelState.IsValid)
            {
                db.Equipes.Add(equipes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement", equipes.IdDep);
            return View(equipes);
        }




    }
}
