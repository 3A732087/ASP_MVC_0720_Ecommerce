using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class HomeWebController : Controller
    {
        private readonly ProductService productService = new ProductService();
        private readonly SlideShowService slideshowService = new SlideShowService();
        // GET: SHOP/HomeWeb
        public ActionResult Index()
        {
            ViewBag.RecommendProducts = productService.GetRecommendProducts();
            ViewBag.SlideShow =  slideshowService.GetAllSlideShow();
            return View();
        }
    }
}