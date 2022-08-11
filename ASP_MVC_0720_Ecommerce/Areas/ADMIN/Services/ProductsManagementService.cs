using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services
{
    public class ProductsManagementService
    {
        //資料庫連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得所有商品資料
        public List<AdminProducts> GetAllProducts()
        {
            string sql = @"SELECT * FROM Products ";
            List<AdminProducts> ProductList = new List<AdminProducts>();

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
                        AdminProducts Data = new AdminProducts();
                        Data.Product_Id = Convert.ToInt32(dr["Product_Id"]);
                        Data.Product_No = dr["Product_No"].ToString();
                        Data.Product_Name = dr["Product_Name"].ToString();
                        Data.Price = Convert.ToInt32(dr["Price"]);
                        Data.Image = dr["Image"].ToString();
                        Data.Recommend = dr["Recommend"].ToString();
                        Data.Quantity = Convert.ToInt32(dr["Quantity"]);
                        ProductList.Add(Data);
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
            return ProductList;
        }
        #endregion

        #region 根據編號取得商品資料
        public AdminProducts GetOneProduct(string Product_No)
        {
            string sql = @"SELECT * FROM Products WHERE Product_No = @Product_No";
            AdminProducts Data = new AdminProducts();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Product_No", SqlDbType.VarChar).Value = Product_No;


                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    Data.Product_Id = Convert.ToInt32(dr["Product_Id"]);
                    Data.Product_No = dr["Product_No"].ToString();
                    Data.Product_Name = dr["Product_Name"].ToString();
                    Data.Product_Content = dr["Product_Content"].ToString();
                    Data.Price = Convert.ToInt32(dr["Price"]);
                    Data.Image = dr["Image"].ToString();
                    Data.Recommend = dr["Recommend"].ToString();
                    Data.Quantity = Convert.ToInt32(dr["Quantity"]);
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

        #region 刪除某筆商品資料
        public void DelProduct(string Product_No)
        {
            string sql = @"DELETE FROM Products WHERE Product_No = @Product_No";
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Product_No", SqlDbType.NVarChar).Value = Product_No;

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

        #region 檢查商品圖片類型
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

        #region 取得新ID值
        public int GetNewId()
        {
            string sql = @"select ident_current('Products')";
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
            catch (Exception e)
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

        #region 新增商品資料
        public void CreateProduct(AdminProducts newProduct)
        {
            string sql = @"INSERT INTO Products (Product_Name, Product_Content, Price, Image, Recommend, Quantity) Values (@Product_Name, @Product_Content, @Price, @Image, @Recommend, @Quantity);SELECT CONVERT(INT, SCOPE_IDENTITY());";


            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Product_Name", SqlDbType.NVarChar).Value = newProduct.Product_Name;
                Sql_cmd.Parameters.Add("@Product_Content", SqlDbType.NVarChar).Value = newProduct.Product_Content;
                Sql_cmd.Parameters.Add("@Price", SqlDbType.Int).Value = newProduct.Price;
                Sql_cmd.Parameters.Add("@Image", SqlDbType.NVarChar).Value = newProduct.Image;
                Sql_cmd.Parameters.Add("@Recommend", SqlDbType.NVarChar).Value = newProduct.Recommend;
                Sql_cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = newProduct.Quantity;

                int pkid = (Int32)Sql_cmd.ExecuteScalar();

                #region 商品編號寫入
                sql = @"UPDATE Products　SET Product_No = @Product_No WHERE Product_Id = @Product_Id";

                string Product_No = "A" + DateTime.Now.ToString("yyyyMMdd") + string.Format("{0:000}", Convert.ToInt32(pkid));

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Product_No", SqlDbType.NVarChar).Value = Product_No;
                Sql_cmd.Parameters.Add("@Product_Id", SqlDbType.Int).Value = pkid;

                Sql_cmd.ExecuteNonQuery();
                #endregion

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


        #region 更新商品資料(編輯)
        public void EditProduct (AdminProducts EditProduct)
        {
            string sql_main = @"UPDATE Products SET Product_Name = @Product_Name, Product_Content = @Product_Content, Price = @Price, Image = @Image, Recommend = @Recommend, Quantity = @Quantity WHERE Product_No = @Product_No";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql_main;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("Product_Name", SqlDbType.NVarChar).Value = EditProduct.Product_Name;
                Sql_cmd.Parameters.Add("Product_Content", SqlDbType.NVarChar).Value = EditProduct.Product_Content;
                Sql_cmd.Parameters.Add("Price", SqlDbType.Int).Value = EditProduct.Price;
                Sql_cmd.Parameters.Add("Image", SqlDbType.NVarChar).Value = EditProduct.Image; 
                Sql_cmd.Parameters.Add("Recommend", SqlDbType.NVarChar).Value = EditProduct.Recommend;
                Sql_cmd.Parameters.Add("Quantity", SqlDbType.Int).Value = EditProduct.Quantity; 
                Sql_cmd.Parameters.Add("Product_No", SqlDbType.NVarChar).Value = EditProduct.Product_No;
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

    }
}