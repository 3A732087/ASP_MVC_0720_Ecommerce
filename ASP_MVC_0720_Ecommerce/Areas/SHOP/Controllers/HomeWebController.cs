using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class HomeWebController : Controller
    {
        // GET: SHOP/HomeWeb
        public ActionResult Index()
        {
            return View();
        }
    }
}