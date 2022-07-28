using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class CartsController : Controller
    {
        private readonly CartsService cartsService = new CartsService();
        // GET: SHOP/Carts
        public ActionResult Index()
        {
            ViewBag.AllCart = cartsService.GetCartProduct(User.Identity.Name);
            return View();
        }

        //[Authorize]
        public ActionResult Put(string Account, int Product_Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Members");
            }
            else
            {
                if (cartsService.CheckInCart(Account, Product_Id))
                {
                    int OldQty;
                    int NewQty;
                    OldQty = cartsService.GetProductInCartQty(Account, Product_Id);
                    NewQty = OldQty + 1;

                    cartsService.AddtoCart(Account, Product_Id, NewQty, true);
                }
                else
                {
                    cartsService.AddtoCart(Account, Product_Id, 1, false);
                }
                TempData["msg"] = "加入成功！";
                return RedirectToAction("Index", "Products");
            }

        }

        //[Authorize]
        public ActionResult RemoveCartProduct(int Cart_Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Members");
            }
            else
            {
                cartsService.RemoveFromCart(Cart_Id);
                TempData["msg"] = "刪除成功！";
                return RedirectToAction("Index", "Carts");
            }
        }
    }
}