using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StagePFE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace StagePFE.Controllers
{
    public class EquipesController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public EquipesController()
        {
        }

        public EquipesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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


        // GET: Equipes

        public ActionResult Index()
        {
            CongesViewModels cv = new CongesViewModels();

            var user = UserManager.FindById(User.Identity.GetUserId());
            int? idequipe = db.Employe
                 .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().IdEquipe;

            ICollection<Employe> team = db.Employe
               .Where(u => u.IdEquipe == idequipe).ToList();



            var demandes = from demande in db.Demandes
                           join employe in db.Employe on demande.EmployeID equals employe.Id
                           join equipe in db.Equipes on employe.IdEquipe equals equipe.Id
                           where equipe.Id == idequipe.Value
                           select demande
                           ;
            cv.Demande = demandes.ToList();
            return View(cv);
        }

        // GET: Equipes/Details/5
        public ActionResult Details(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipes equipes = db.Equipes.Find(id);
            if (equipes == null)
            {
                return HttpNotFound();
            }
            return View(equipes);
        }

        // GET: Equipes/Create
        public ActionResult Create()
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

        // GET: Equipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipes equipes = db.Equipes.Find(id);
            if (equipes == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement", equipes.IdDep);
            return View(equipes);
        }

        // POST: Equipes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdChef,IdMembre,Departement,IdDep")] Equipes equipes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDep = new SelectList(db.Departements, "Id", "NomDepartement", equipes.IdDep);
            return View(equipes);
        }

        // GET: Equipes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipes equipes = db.Equipes.Find(id);
            if (equipes == null)
            {
                return HttpNotFound();
            }
            return View(equipes);
        }

        // POST: Equipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipes equipes = db.Equipes.Find(id);
            db.Equipes.Remove(equipes);
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
