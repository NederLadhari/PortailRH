using System.Web.Mvc;

namespace StagePFE.Areas.ChefProjet
{
    public class ChefProjetAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ChefProjet";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ChefProjet_default",
                "ChefProjet/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }

                //namespaces: new[] { "StagePFE.Controllers" }
            );

            
        }
    }
}