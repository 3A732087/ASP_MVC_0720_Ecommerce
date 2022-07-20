using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.HOMEWEB
{
    public class HOMEWEBAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HOMEWEB";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HOMEWEB_default",
                "HOMEWEB/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}