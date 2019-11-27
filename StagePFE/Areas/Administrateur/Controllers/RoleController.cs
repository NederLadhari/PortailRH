using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.EntityFramework;
using StagePFE.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using StagePFE.Areas.Administrateur.Models;
using System.Threading.Tasks;
using System.Net;

namespace StagePFE.Areas.Administrateur.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        private CongesEntities db = new CongesEntities();

        public RoleController()
        {
        }

        public RoleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationDbContext _context)
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


        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Roles
        public ActionResult Index()
        {
            return View(context.Roles.ToList());
        }

        public ActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                context.Roles.Add(new IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully !";
                return View("Create");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Create");
        }

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }



        }

        #region Assign Role
        public  ActionResult AssignRole()
        {
            return View();
        }


        [ActionName("AssignRole"), HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRoleAsync(AssignUserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();
            var role = roleManager.FindByIdAsync(model.Role);
            //var role = await RoleManagerExtensions.FindByName(RoleManager<> ,model.Role);
            
            //var user = await UserManager.FindByEmailAsync(model.Email);
            //var role = await RoleManager.FindByNameAsync(model.Role);
            if (user == null)
            {
                ModelState.AddModelError("Email",
                    $"No user found with the email address '{model.Email}'!");
            }
            else if (role == null)
            {
                ModelState.AddModelError("Role",
                    $"The role '{model.Role}' does not exist!");
            }
            //else
            //{ 
            //    UserManager.AddToRoleAsync(user.Id, role.Id);
            //    //await UserManager.AddToRoleAsync(user.Id, role.Name);
            //    TempData["Message"] = $"User '{user.Email}' has been assigned to the role '{role.Name}'!";
            //    return RedirectToAction("Index");
            //}

            return View(model);
        }
        #endregion Assign Role


        public ActionResult TypeConge()
        {
            IEnumerable<Types> type = db.Types
                     
                     .ToList();

            CongesViewModels cv = new CongesViewModels();
            cv.TypeConge = type;

            

            return View(cv);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult AjouterType(String Nom, String JourAlloue)
        {
            if (JourAlloue == null)
            {
                throw new ArgumentNullException(nameof(JourAlloue));
            }

            if (ModelState.IsValid)
            {

           
            var t = new Types
            {
                Nom = Nom,
                JoursAlloues = Convert.ToInt32(JourAlloue)
              
            };

            db.Types.Add(t);
            db.SaveChanges();
            return RedirectToAction("TypeConge", "Role");
            }
            return View();
        }


        public ActionResult SupprimerType(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Types types = db.Types.Find(id);
            if (types == null)
            {
                return HttpNotFound();
            }

            db.Types.Remove(types);
            db.SaveChanges();
            return RedirectToAction("TypeConge", "Role", new { area = "Administrateur" });
        }


        public ActionResult Modification(int? id, String Nom, String JourAlloue)
        {
            Types types = db.Types.Find(id);
            if (types == null)
            {
                return HttpNotFound();
            }
            

           
            types.Nom = Nom;
            types.JoursAlloues = Convert.ToInt32(JourAlloue);
            db.SaveChanges();

            return RedirectToAction("TypeConge", "Role", new { area = "Administrateur" });
        }


    }
}