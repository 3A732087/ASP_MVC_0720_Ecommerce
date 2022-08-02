using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels
{
    public class OrderViewModel
    {
        public List<Order> OrderList { get; set; }
    }
}