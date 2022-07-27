using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Models
{
    public class Products
    {
        public int Product_Id { get; set; }

        [DisplayName("商品編號")]
        public string Product_No { get; set; }

        [DisplayName("商品名稱")]
        [Required(ErrorMessage ="請輸入商品名稱")]
        [StringLength(50, ErrorMessage = "商品名稱不可大於50字元")]
        public string Product_Name { get; set; }

        [DisplayName("商品內容")]
        [Required(ErrorMessage = "請輸入商品內容")]
        public string Product_Content { get; set; }

        [DisplayName("價格")]
        [Required(ErrorMessage = "請輸入價格")]
        [Range(typeof(int), "1", "9999", ErrorMessage = "價格請介於1-9999")]
        public int Price { get; set; }

        [DisplayName("安全銷售量")]
        [Required(ErrorMessage = "安全銷售量")]
        [Range(typeof(int), "1", "999", ErrorMessage = "安全銷售量請介於1-999")]
        public int Quantity { get; set; }

        [DisplayName("商品圖片")]
        public string Image { get; set; }

        public string Recommend { get; set; }
    }
}