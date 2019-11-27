using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StagePFE.Areas.Ingenieur.Controllers
{
    public class CalendrierController : Controller
    {
        private ApplicationUserManager _userManager;
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

        private CongesEntities db = new CongesEntities();
        // GET: Ingenieur/Calendrier

        //    public ActionResult getCalendar()
        //{
        //    var data = db.Demandes.AsEnumerable().Select(p => new
        //    {
        //        DateDebut = p.DateDebut.Value.ToString(),
        //        DateFin = p.DateFin.Value.ToString(),
        //        Motif = p.Motif,
        //        Etat =p.Etat
        //    }).Where(x=>x.Etat=="Accepté")
        //    .ToList();

        //    return Json(data, JsonRequestBehavior.AllowGet);
            
        //}

        public ActionResult CalendrierIngenieur()
        {
            CongesViewModels cv = new CongesViewModels();

            var user = UserManager.FindById(User.Identity.GetUserId());
            int? idequipe = db.Employe
                 .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().IdEquipe;


            var demandes = from demande in db.Demandes
                           join employe in db.Employe on demande.EmployeID equals employe.Id

                           where employe.IdUser == user.Id
                           where demande.Etat == "Accepté"
                           select demande
                           ;
            cv.Demande = demandes.ToList();

            
            cv.Demande = demandes.ToList();
            return View(cv);
        }
    }
}