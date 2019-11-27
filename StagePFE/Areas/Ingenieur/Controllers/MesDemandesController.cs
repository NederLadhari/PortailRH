  
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace StagePFE.Areas.Ingenieur.Controllers
{
    public class MesDemandesController : Controller
        {
            private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;
            private ApplicationDbContext _context;
            private CongesEntities db = new CongesEntities();

            public MesDemandesController() { }

            public MesDemandesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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


            // GET: DemandeConge
            public ActionResult MesDemandes()
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                int idEmploye = db.Employe
                      .Where(u => u.IdUser == user.Id)
                      .FirstOrDefault().Id;

                IEnumerable<Demandes> demande = db.Demandes
                      .Where(u => u.EmployeID == idEmploye)
                      .Where(x => x.Etat == "En cours de traitement" || x.Etat == "En cours de validation")

                      .ToList();

                CongesViewModels cv = new CongesViewModels();
                cv.Demande = demande;

            ViewBag.TypeId = new SelectList(db.Types, "Id", "Nom");

                return View(cv); 
            }

        public ActionResult Historique()
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


        public ActionResult Accueil()
            {
                return View();
            }

            public ActionResult AjouterConge()
            {
                return View();
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


            if (ModelState.IsValid)
            {

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

            DateTime fin  = DateTime.Parse(DateFin, MyCultureInfo);
            
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

           DateTime start= DateTime.ParseExact(DateDebut,
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
                        DateDebut =start,
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
                  //return RedirectToAction("MesDemandes", "MesDemandes");
                }
            ModelState.AddModelError("", "Tentative de connexion non valide.");
            return RedirectToAction("MesDemandes", "MesDemandes");
            //}


            //return View(model);
        }

       
        public ActionResult Test() {


            return View();
        }



        public JsonResult SendMailToUser()
        {
            bool result = false;
            result = SendEmail("zoukarahlem@gmail.com", "First test", "<p> it works </p>");
            return Json(result, JsonRequestBehavior.AllowGet);



        }



        public bool SendEmail(String ToEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["Senderpassword"].ToString();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                MailMessage mailMessage = new MailMessage(senderEmail, ToEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }




        public ActionResult SupprimerDemande(int? id)
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
            return RedirectToAction("MesDemandes", "MesDemandes", new { area = "Ingenieur" });
        }


        public ActionResult Modification(int? id, String DateDebut, String DateFin, String motif, String Type)
        {
            Demandes demandes = db.Demandes.Find(id);
            if (demandes == null)
            {
                return HttpNotFound();
            }

            int idtype = db.Types
            .Where(u => u.Nom == Type)
             .FirstOrDefault().Id;

            DateTime start = DateTime.ParseExact(DateDebut,
                   new[] {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                        "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"},
                   CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime end = DateTime.ParseExact(DateFin,
                    new[] {"dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd",
                        "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy"},
                    CultureInfo.InvariantCulture, DateTimeStyles.None);
            demandes.DateDebut = start;
           demandes.DateFin = end;
            demandes.Motif = motif;
            demandes.TypeID = idtype;
            db.SaveChanges();

            return RedirectToAction("MesDemandes", "MesDemandes", new { area = "Ingenieur" });
        }



        public ActionResult ModifierDemande(int? id, String DateDebut, String DateFin, String motif, String Types )
        {
            
            Demandes demandes = db.Demandes.Find(id);
            if (demandes == null)
            {
                return HttpNotFound();
            }
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            DateTime Debut = DateTime.Parse(DateDebut, MyCultureInfo);

            DateTime fin = DateTime.Parse(DateFin, MyCultureInfo);
            demandes.DateDebut = Debut;
            demandes.DateFin = fin;
            demandes.Motif = motif;
            db.SaveChanges();

            return RedirectToAction("MesDemandes", "MesDemandes", new { area = "Ingenieur" });
        }
       
        




    public ActionResult dem ()
        {
            return View();
        }

    }
    }