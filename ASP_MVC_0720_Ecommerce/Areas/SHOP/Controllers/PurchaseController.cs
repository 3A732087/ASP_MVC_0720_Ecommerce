﻿using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using ASP_MVC_0720_Ecommerce.Areas.SHOP.Services;
using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using ASP_MVC_0720_Ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly CartsService cartsService = new CartsService();

        // GET: SHOP/Purchase
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Members");
            }
            else
            {
                int CartQty = cartsService.GetCartProduct(User.Identity.Name).Count();
                if(CartQty == 0)
                {
                    TempData["msg"] = "購物車內尚無商品！";
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    CheckoutViewModel Data = new CheckoutViewModel();
                    Data.Product = cartsService.GetCartProduct(User.Identity.Name);
                    return View(Data);
                }
            }
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutViewModel CheckoutData)
        {
            //判斷頁面資料是否都經過驗證(Model)
            if (ModelState.IsValid)
            {
                PurchaseSerivce purchaseService = new PurchaseSerivce();
                LineNotifyService linenotifyService = new LineNotifyService();
                MembersDBService membersDBService = new MembersDBService();

                CheckoutData.Product = cartsService.GetCartProduct(User.Identity.Name);
                CheckoutData.newOrder.Account = User.Identity.Name;
                Dictionary<string,object> _Result = purchaseService.CheckoutAll(CheckoutData);
                if(_Result["Result"].Equals(true))
                {
                    TempData["msg"] = "結帳完成！";
                    string AccessToken = membersDBService.GetLineNotifyAccessToken(User.Identity.Name);
                    linenotifyService.LineMsgPush(AccessToken, _Result);
                }
                return RedirectToAction("Index", "HomeWeb", new { area = "SHOP" });
            }
            else
            {
                return View();
            }
        }
    }
}