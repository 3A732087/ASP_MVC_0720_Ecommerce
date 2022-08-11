using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models
{
    public class AdminSlideShow
    {
        public int Img_Id { get; set; }

        [DisplayName("輪播圖片")]
        public string Img_Name { get; set; }

        [DisplayName("輪播圖片")]
        [Required(ErrorMessage = "請上傳輪播圖片")]
        public HttpPostedFileBase SlideShowImg { get; set; }
    }
}