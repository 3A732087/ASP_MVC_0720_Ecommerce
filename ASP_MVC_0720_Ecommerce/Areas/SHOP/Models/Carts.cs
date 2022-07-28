using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Models
{
    public class Carts
    {
        [DisplayName("購物車編號")]
        public int Cart_Id { get; set; }

        [DisplayName("帳號")]
        public string Account { get; set; }

        public int Product_Id { get; set; }

        [DisplayName("數量")]
        public int Qty { get; set; }

        public Products Product { get; set; } = new Products();
    }
}