using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Models
{
    public class SlideShow
    {
        public int Img_Id { get; set; }

        [DisplayName("輪播圖片")]
        public string Img_Name { get; set; }
    }
}