using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services
{
    public class SlideShowManagementService
    {
        //連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得所有輪播圖資料
        public List<AdminSlideShow> GetAllSlideShow()
        {
            string sql = @"SELECT * FROM SlideShow ";
            List<AdminSlideShow> SlideShowList = new List<AdminSlideShow>();

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
                        AdminSlideShow Data = new AdminSlideShow();
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

        #region 根據編號取得輪播圖資料
        public AdminSlideShow GetOneSlideShowImg(string Img_Id)
        {
            string sql = @"SELECT * FROM SlideShow WHERE Img_Id = @Img_Id";
            AdminSlideShow Data = new AdminSlideShow();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Img_Id", SqlDbType.Int).Value = Img_Id;


                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    Data.Img_Id = Convert.ToInt32(dr["Img_Id"]);
                    Data.Img_Name = dr["Img_Name"].ToString();
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
            return Data;
        }
        #endregion

        #region 刪除某張輪播圖
        public void DelSlideShowImg(string Img_Id)
        {
            string sql = @"DELETE FROM SlideShow WHERE Img_Id = @Img_Id";
            try
            {   
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Img_Id", SqlDbType.Int).Value = Img_Id;

                Sql_cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 取得新ID值
        public int GetNewId()
        {
            string sql = @"select ident_current('SlideShow')";
            int newId = 0;
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;
                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    newId = Convert.ToInt32(dr[0]) + 1;
                }

            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return newId;
        }
        #endregion

        #region 新增輪播圖資料
        public void CreateSlideShow(AdminSlideShow newSlideShow)
        {
            string sql = @"INSERT INTO SlideShow (Img_Name) Values (@Img_Name)";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Img_Name", SqlDbType.NVarChar).Value = newSlideShow.Img_Name;
                Sql_cmd.ExecuteNonQuery();                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 更新輪播圖資料(編輯)
        public void EditSlideShow(AdminSlideShow EditSlideShow)
        {
            string sql_main = @"UPDATE SlideShow SET Img_Name = @Img_Name WHERE Img_Id = @Img_Id";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql_main;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("Img_Name", SqlDbType.NVarChar).Value = EditSlideShow.Img_Name;
                Sql_cmd.Parameters.Add("Img_Id", SqlDbType.Int).Value = EditSlideShow.Img_Id;
                Sql_cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImg(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion

    }
}