using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Services
{
    public class LineNotifyService
    {
        //連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 推送訊息
        public void LineMsgPush(string AccessToken, Dictionary<string, object> Data)
        {
            string PUSH_MSG = "\n您的訂單編號" + Data["Order_No"].ToString() + "已收到" +
                "\n感謝您的購買！";

            string Url = "https://notify-api.line.me/api/notify";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            string content = "";
            content += "message=\r\n" + PUSH_MSG;

            if (!string.IsNullOrEmpty(AccessToken))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                using(var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
        }
        #endregion
    }
}