using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Services
{
    public class SlideShowService
    {        
        
        //資料庫連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得所有輪播圖資料
        public List<SlideShow> GetAllSlideShow()
        {
            string sql = @"SELECT * FROM SlideShow ";
            List<SlideShow> SlideShowList = new List<SlideShow>();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;
                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SlideShow Data = new SlideShow();
                        Data.Img_Id = Convert.ToInt32(dr["Img_Id"]);
                        Data.Img_Name = dr["Img_Name"].ToString();
                        SlideShowList.Add(Data);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return SlideShowList;
        }
        #endregion

    }
}