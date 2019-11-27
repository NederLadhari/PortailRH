using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.RH.Controllers
{
    public class DemandeRHController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();
        public DemandeRHController()
        {
        }

        public DemandeRHController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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

        public ActionResult DemandeRH()
        {
            CongesViewModels cv = new CongesViewModels();

            var demandess = from demande in db.Demandes
                            join employe in db.Employe on demande.EmployeID equals employe.Id

                            where demande.ApprobationChef == "Accepté"
                            where demande.Etat == "En cours de validation"
                            
                            select demande
                           ;
            cv.Demande = demandess.ToList();
            return View(cv);
        }


        public ActionResult AccepterDemande(int? id, int? emp, int nbr)
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
            demandes.Etat = "Accepté";
            db.SaveChanges();

           
            if (emp == null)
            {
                return HttpNotFound();
            }
            Compte compte = db.Compte
                  .Where(u => u.IdEmploye == emp)
                  .FirstOrDefault();
            var solde = db.Compte
                .Where(u => u.IdEmploye == emp)
                .FirstOrDefault().Solde;
            var idd = db.Employe
                  .Where(u => u.Id == emp)
                  .FirstOrDefault().IdUser;
            compte.Solde = solde - nbr;
            var email = db.AspNetUsers.Where(u => u.Id == idd).FirstOrDefault().Email;
            db.SaveChanges();
            bool result = false;
            result = SendEmail(email, "Réponse de demande", "<p>Votre demande a été accepté </p>");
            //return Json(result, JsonRequestBehavior.AllowGet);

            return RedirectToAction("DemandeRH", new { area = "RH" });


            //Historique



            //return RedirectToAction("EnAttente", "Demandes");
        }

        public ActionResult RejetterDemande(int? id, int? emp)
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
            var idd = db.Employe
                 .Where(u => u.Id == emp)
                 .FirstOrDefault().IdUser;
            var email = db.AspNetUsers.Where(u => u.Id == idd).FirstOrDefault().Email;
            db.SaveChanges();
           
            
            bool result = false;
            result = SendEmail(email, "Réponse de demande", "<p> Votre Demande a été rejettée </p>");
            //return Json(result, JsonRequestBehavior.AllowGet);
            return RedirectToAction("DemandeRH", new { area = "Administrateur" });
        }


        public JsonResult SendMailToUser()
        {
            bool result = false;
            result = SendEmail("zoukarahlem@gmail.com", "Réponse de demande", "<p> it works </p>");
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

    }
}