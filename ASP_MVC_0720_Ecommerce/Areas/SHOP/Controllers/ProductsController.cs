using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService productService = new ProductService();
        // GET: SHOP/Products
        public ActionResult Index()
        {
            ViewBag.ProductList = productService.GetAllProducts();
            return View();
        }

        public ActionResult Product(int ProductId)
        {
            ViewBag.Product = productService.GetDataById(ProductId);
            return View();
        }
    }
}