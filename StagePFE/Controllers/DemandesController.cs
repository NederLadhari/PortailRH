using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Controllers
{
    public class DemandesController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public DemandesController()
        {
        }

        public DemandesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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


        // GET: Demandes
        public ActionResult AjouterConge()
        {
            

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AjouterConge(CongesViewModels model)
        {
            //GET: ID Employe
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;


            //if (ModelState.IsValid)
            //{

            //GET:Id Type
            var SelectedType = model.NomType;

            int idtype = db.Types
              .Where(u => u.Nom == SelectedType.ToString())
              .FirstOrDefault().Id;

            // GET: Nombre de jours alloué
            int? jours = db.Types
              .Where(u => u.Nom == SelectedType.ToString())
              .FirstOrDefault().JoursAlloues;

            //GET: Solde Employé
            var solde = db.Compte
              .Where(u => u.IdEmploye == idEmploye)
              .FirstOrDefault().Solde;



            if (solde >= model.Nombre)
            {
                DateTime today = DateTime.Today;
                var d = new Demandes
                {
                    DateDebut = model.DateDebut,
                    DateFin = model.DateFin,
                    DateDemande = today,
                    TypeID = idtype,
                    Motif = model.Motif,
                    NombreJours = model.Nombre,
                    EmployeID = idEmploye


                };

                db.Demandes.Add(d);
                db.SaveChanges();
                return RedirectToAction("MesDemandes", "Demandes");
            }

            TempData["msg"] = "<script>alert('Solde insuffisant');</script>";
            //}


            return View();
        }

        public ActionResult ModifierDemande()
        {


            return View();
        }

        public ActionResult EffacerDemande()
        {
            return View();
        }
     
       

        //GET:Demande Utilisateur

        public ActionResult MesDemandes()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(u => u.EmployeID == idEmploye).ToList();
            CongesViewModels cv = new CongesViewModels();
            cv.Demande = demande;
            


            return View(cv);
        }

        public ActionResult EnAttente()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int idEmploye = db.Employe
                  .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().Id;

            IEnumerable<Demandes> demande = db.Demandes
                  .Where(u => u.ApprobationChef == "Accepté").ToList();
            CongesViewModels cv = new CongesViewModels();
            cv.Demande = demande;



            return View(cv);
        }

        // Approuver une demande par l'admin
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
            { return HttpNotFound();
            }
            Compte compte = db.Compte
                  .Where(u => u.IdEmploye== employe.Id)
                  .FirstOrDefault();
            var solde = db.Compte
                .Where(u => u.IdEmploye == employe.Id)
                .FirstOrDefault().Solde;

            compte.Solde = solde - nbr;
            
            db.SaveChanges();
           
            return RedirectToAction("EnAttente", "Demandes");

           
            //Historique



            //return RedirectToAction("EnAttente", "Demandes");
        }

        //Rejetter une demande par l'admin

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
            demandes.Etat = "Rejetter";
            db.SaveChanges();
            return RedirectToAction("EnAttente", "Demandes");
        }





    //    public ActionResult Contact(ContactViewModel contactViewModel)
    //    {

    //        if (ModelState.IsValid)
    //        {

    //            try
    //            {

    //                MailMessage msz = new MailMessage();
    //                msz.From = new MailAddress("nasnes21@gmail.com");//Email which you are getting 
    //                                                                   //from contact us page 
    //                msz.To.Add("zoukarahlem@gmail.com");//Where mail will be sent 
    //                msz.Subject = "Congé";
    //                msz.Body = "Accepté";
    //                SmtpClient smtp = new SmtpClient();

    //                smtp.Host = "smtp.gmail.com";

    //                smtp.Port = 587;

    //                smtp.Credentials = new System.Net.NetworkCredential
    //                ("", "");

    //                smtp.EnableSsl = true;

    //                smtp.Send(msz);
    //                ModelState.Clear();


    //                return Json(new { statut = "ok", message = "votre message a été envoyé" });
    //            }


    //            catch (Exception ex)
    //            {

    //                return Json(new { statut = "ko", message = "votre message n'a pas été envoyé" });
    //            }
    //        }

    //        return View(contactViewModel);
    //    }

    //}








}
}