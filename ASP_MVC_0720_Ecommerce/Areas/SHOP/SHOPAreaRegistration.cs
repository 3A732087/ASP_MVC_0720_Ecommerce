using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP
{
    public class SHOPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SHOP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SHOP_default",
                "SHOP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}