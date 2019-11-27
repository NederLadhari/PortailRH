using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.ChefProjet.Controllers
{
    public class TraiterDemandeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public TraiterDemandeController()
        {
        }

        public TraiterDemandeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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


        public ActionResult MesDemandes()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(u => u.EmployeID == idEmploye)
                  .Where(x => x.Etat == "En cours de Traitement" || x.Etat == "En cours de validation")
                  .ToList();
            CongesViewModels cv = new CongesViewModels();
            cv.Demande = demande;



            return View(cv);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult AjouterConge(String DateDebut, String DateFin, String motif, String Type)
        {
            //GET: ID Employe
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;


            //if (ModelState.IsValid)
            //{

            //GET:Id Type
            // var SelectedType = model.NomType;

            int idtype = db.Types
             .Where(u => u.Nom == Type)
              .FirstOrDefault().Id;

            // GET: Nombre de jours alloué
            int? jours = db.Types
              .Where(u => u.Nom == Type)
              .FirstOrDefault().JoursAlloues;

            //GET: Solde Employé
            var solde = db.Compte
                  .Where(u => u.IdEmploye == idEmploye)
                  .FirstOrDefault().Solde;

            //Nombre de jour
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            DateTime debut = DateTime.Parse(DateDebut, MyCultureInfo);

            DateTime fin = DateTime.Parse(DateFin, MyCultureInfo);

            //TimeSpan Difference = fin - debut;
            int amount = 0;
            int sdayIndex = (int)debut.DayOfWeek;
            int edayIndex = (int)fin.DayOfWeek;
            //var days = Difference.TotalDays;
            amount += (sdayIndex == 0) ? 5 : (6 - sdayIndex);
            amount += (edayIndex == 6) ? 5 : edayIndex;


            debut = debut.AddDays(7 - sdayIndex);
            fin = fin.AddDays(-edayIndex);


            if (debut > fin)
                amount -= 5;
            else
                amount += (fin.Subtract(debut)).Days / 7 * 5;

            DateTime start = DateTime.ParseExact(DateDebut,
                   new[] {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                        "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"},
                   CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime end = DateTime.ParseExact(DateFin,
                    new[] {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                        "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"},
                    CultureInfo.InvariantCulture, DateTimeStyles.None);

            //if (solde >= model.Nombre)
            //{
            DateTime today = DateTime.Today;
            var d = new Demandes
            {
                DateDebut = start,
                DateFin = end,
                DateDemande = today,
                TypeID = idtype,
                Motif = motif,
                NombreJours = amount,
                EmployeID = idEmploye,
                Etat = "En cours de traitement"

            };

            db.Demandes.Add(d);
            db.SaveChanges();
            return RedirectToAction("MesDemandes", "TraiterDemande", new { area = "ChefProjet" });
            //}

            //return View("Lockout");
            //}


            //return View(model);
        }


        // GET: ChefProjet/Demandes
        public ActionResult EnAttente()
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
                           where demande.Etat=="En cours de Traitement"
                           select demande
                           ;
            cv.Demande = demandes.ToList();
            return View(cv);


        }

        

        // GET: AccepterDemande/5
        public ActionResult AccepterDemande(int? id)
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
           
            demandes.ApprobationChef = "Accepté";
            demandes.Etat = "En cours de validation";
            db.SaveChanges();
            return RedirectToAction("EnAttente", "TraiterDemande");
        }

        // GET: RefuserDemande/5
        public ActionResult RefuserDemande(int? id)
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
            
            demandes.ApprobationChef = "Refusé";
            db.SaveChanges();
            return RedirectToAction("EnAttente", "TraiterDemande");
        }

        public ActionResult DetailDemande(int? id)
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

            int? idemploye = db.Employe
                 .Where(u => u.Id == demandes.EmployeID)
                  .FirstOrDefault().Id;
            CongesViewModels cv = new CongesViewModels();

            var demandess = from demande in db.Demandes
                           join employe in db.Employe on demande.EmployeID equals employe.Id
                           
                           where employe.Id == idemploye.Value
                           select demande
                           ;
            cv.Demande = demandess.ToList();
            return View(cv);

            
        }

        public ActionResult SupprimerDemandeChef(int? id)
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

            db.Demandes.Remove(demandes);
            db.SaveChanges();
            return RedirectToAction("MesDemandes", "TraiterDemande", new { area = "ChefProjet" });
        }

        public ActionResult Modification(int? id, String DateDebut, String DateFin, String motif, String Types)
        {
            Demandes demandes = db.Demandes.Find(id);
            if (demandes == null)
            {
                return HttpNotFound();
            }

            int idtype = db.Types
            .Where(u => u.Nom == Types)
             .FirstOrDefault().Id;


            demandes.DateDebut = DateTime.Parse(DateDebut);
            demandes.DateFin = DateTime.Parse(DateFin);
            demandes.Motif = motif;
            demandes.TypeID = idtype;
            db.SaveChanges();

            return RedirectToAction("MesDemandes", "TraiterDemande", new { area = "ChefProjet" });
        }

        public ActionResult ModifierDemandeChef(int? id)
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


            return View(demandes)
            ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifierDemande([Bind(Include = "Id,Nom,Prenom,Poste,DateRecrutement,DateNaissance,Sexe,Telephone,Adresse,Cin,IdUser,IdEquipe")] Demandes demandes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(demandes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", employe.IdUser);
            return View(demandes);
        }

        public ActionResult HistoriqueChef()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(u => u.EmployeID == idEmploye)
                  .Where(x => x.Etat == "Accepté" || x.Etat == "Refusé")
                  .ToList();
            CongesViewModels cv = new CongesViewModels();
            cv.Demande = demande;
            return View(cv);
        }

    }

    
}