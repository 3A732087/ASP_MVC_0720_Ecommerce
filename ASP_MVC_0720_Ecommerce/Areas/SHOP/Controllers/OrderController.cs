using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class OrderController : Controller
    {
        // GET: SHOP/Order
        //public ActionResult Index()
        //{
        //    return View();
        //}

        private readonly OrderService orderService = new OrderService();

        //訂單主檔
        public ActionResult Main()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Members");
            }
            else
            {
                OrderViewModel Data = new OrderViewModel();
                Data.OrderList = orderService.GetOrderList(User.Identity.Name);
                return View(Data);
            }
        }

        //訂單明細
        public ActionResult OrderDetail(string OrderNo)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Members");
            }
            else
            {
                OrderDetailViewModel Data = new OrderDetailViewModel();
                Data.DetailData = orderService.GetOrderDetail(User.Identity.Name, OrderNo);
                return View(Data);
            }
        }
    }
}