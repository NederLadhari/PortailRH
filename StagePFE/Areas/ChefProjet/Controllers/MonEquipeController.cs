using StagePFE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace StagePFE.Areas.ChefProjet.Controllers
{
    

    public class MonEquipeController : Controller
    {
        private ApplicationUserManager _userManager;
        private CongesEntities db = new CongesEntities();

        // GET: ChefProjet/MonEquipe

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


        public ActionResult ListeEquipe()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            int? idequipe = db.Employe
                 .Where(u => u.IdUser == user.Id)
                  .FirstOrDefault().IdEquipe;

            var employe = db.Employe.Include(e => e.AspNetUsers).Where(x => x.IdEquipe==idequipe);
            return View(employe.ToList());
        }
    }
}