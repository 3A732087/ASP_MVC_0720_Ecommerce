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
    public class AdminManagementController : Controller
    {
        MembersManagementService membersmanagementService = new MembersManagementService();

        // GET: ADMIN/AdminManagement
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Members()
        {
            AllMembersViewModel allMebers = new AllMembersViewModel();
            allMebers.AllMembersData = membersmanagementService.GetAllMembers();
            return View(allMebers);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DelMember(string Account)
        {
            membersmanagementService.DeleteMember(Account);
            return RedirectToAction("Members", "AdminManagement", new { area = "ADMIN" });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditMember(string Account)
        {
            AdminMembersViewModel memberData = new AdminMembersViewModel();
            memberData.EditMember = membersmanagementService.GetMemberByAccount(Account);

            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem { Text = "是", Value = "1" });
            Data.Add(new SelectListItem { Text = "否", Value = "0", Selected = true });

            memberData.IsOnList = Data;

            return View(memberData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditMember(AdminMembersViewModel Data)
        {
            membersmanagementService.EditMember(Data.EditMember);
            TempData["msg"] = "修改成功！";

            return RedirectToAction("Members", "AdminManagement");
        }


    }
}