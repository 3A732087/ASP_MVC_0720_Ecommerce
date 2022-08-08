using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Controllers
{
    public class AdminOrdersManagementController : Controller
    {
        OrderManagementService ordermanagentService = new OrderManagementService();

        [Authorize(Roles = "Admin")]
        public ActionResult Orders()
        {
            AllOrdersViewModel Data = new AllOrdersViewModel();
            Data.OrderList = ordermanagentService.GetAllOrderList();

            return View(Data);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditOrders(string Order_No)
        {
            AdminOrderDetailViewModel DetailData = new AdminOrderDetailViewModel();

            DetailData.DetailData = ordermanagentService.GetOrderDetail(Order_No);
            return View(DetailData);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditOrders(AdminOrderDetailViewModel EditOrder)
        {
            ordermanagentService.EditOrder(EditOrder);
            return RedirectToAction("Orders", "AdminOrdersManagement");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DelOrders(string Order_No)
        {
            ordermanagentService.DelOrder(Order_No);
            TempData["msg"] = "刪除成功！";
            return RedirectToAction("Orders", "AdminOrdersManagement");
        }
    }
}