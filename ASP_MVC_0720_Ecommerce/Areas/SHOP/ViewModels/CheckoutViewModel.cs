using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels
{
    public class CheckoutViewModel
    {
        public Order newOrder { get; set; } = new Order();

        public int Product_Id { get; set; }

        public List<Carts> Product { get; set; }

        [DisplayName("數量")]
        public int Qty { get; set; }
    }
}