using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.ViewModel
{
    public class AdminMembersViewModel
    {
        public AdminMembers EditMember { get; set; }
        public List<SelectListItem> IsOnList { get; set; }
    }
}