using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models
{
    public class AdminMembers
    {
        [DisplayName("帳號")]
        public string Account { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }

        [DisplayName("姓名")]
        [Required(ErrorMessage = "請輸入姓名")]
        [StringLength(20, ErrorMessage = "姓名長度最多20字元")]
        public string Name { get; set; }

        [DisplayName("電子郵件")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email長度最多200字元")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string Email { get; set; }

        [DisplayName("授權碼")]
        public string AuthCode { get; set; }

        [DisplayName("是否為管理員")]
        [Required(ErrorMessage = "請設定是否為管理員")]
        public string IsAdmin { get; set; }

        [DisplayName("LineNotifyAccessToken")]
        public string LineNotifyAccessToken { get; set; }
    }
}