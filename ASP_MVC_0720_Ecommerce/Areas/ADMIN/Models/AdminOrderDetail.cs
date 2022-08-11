﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models
{
    public class AdminOrderDetail
    {
        [DisplayName("訂單明細編號")]
        public int OrderDetail_Id { get; set; }

        [DisplayName("訂單編號")]
        public string Order_No { get; set; }

        [DisplayName("帳號")]
        public string Account { get; set; }

        [DisplayName("收件人")]
        [Required(ErrorMessage = "請輸入收件人")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "此帳號長度需介於1-10字元")]
        public string Receiver { get; set; }

        [DisplayName("電子郵件")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email長度最多200字元")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string Email { get; set; }

        [DisplayName("地址")]
        [Required(ErrorMessage = "請輸入地址")]
        [StringLength(100, ErrorMessage = "地址長度最多100字元")]
        public string Address { get; set; }

        [DisplayName("訂單日期")]
        public DateTime Date { get; set; }

        [DisplayName("總金額")]
        public int Total { get; set; }

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