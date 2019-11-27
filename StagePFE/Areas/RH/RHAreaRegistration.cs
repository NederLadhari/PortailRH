using System.Web.Mvc;

namespace StagePFE.Areas.RH
{
    public class RHAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RH";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RH_default",
                "RH/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}