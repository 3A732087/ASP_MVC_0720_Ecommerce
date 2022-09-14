using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using PagedList;
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

        public ActionResult Index(int? page)
        {
            var ProductsData = productService.GetAllProducts();

            var pageNumber = page ?? 1;
            var onePageOfProducts = ProductsData.ToPagedList(pageNumber, 5);
            ViewBag.ProductList = onePageOfProducts;

            return View();
        }

        public ActionResult Product(int ProductId)
        {
            ViewBag.Product = productService.GetDataById(ProductId);
            return View();
        }
    }
}