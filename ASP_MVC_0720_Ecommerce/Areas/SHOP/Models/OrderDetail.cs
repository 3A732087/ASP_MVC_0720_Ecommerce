using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Models
{
    public class OrderDetail
    {
        [DisplayName("訂單編號")]
        public string Order_No { get; set; }

        [DisplayName("帳號")]
        public string Account { get; set; }

        [DisplayName("產品編號")]
        public string Product_No { get; set; }

        [DisplayName("產品名稱")]
        public string Product_Name { get; set; }

        [DisplayName("價格")]
        public int Price { get; set; }

        [DisplayName("數量")]
        public int Qty { get; set; }

    }
}