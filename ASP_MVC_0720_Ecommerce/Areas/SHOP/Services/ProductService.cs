using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Services
{
    public class ProductService
    {
        //資料庫連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //建立與資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單一商品資料
        public Products GetDataById(int Id)
        {
            //回傳根據編號所取得的資料
            Products Data = new Products();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM PRODUCTS WHERE Product_Id = @Id";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("Id", SqlDbType.Int).Value = Id;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                dr.Read();
                Data.Product_Id = Convert.ToInt32(dr["Product_Id"]);
                Data.Product_No = dr["Product_Id"].ToString();
                Data.Product_Name = dr["Product_Name"].ToString();
                Data.Product_Content = dr["Product_Content"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
                Data.Quantity = Convert.ToInt32(dr["Quantity"]);
                Data.Image = dr["Image"].ToString();
                Data.Recommend = dr["Recommend"].ToString();

            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 取得所有商品
        public List<Products> GetAllProducts()
        {
            List<Products> ProductList = new List<Products>();
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"select * from Products";
                Sql_cmd.CommandText = sql;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Products product = new Products
                        {
                            Product_Id = Convert.ToInt32(dr["Product_Id"]),
                            Product_No = dr["Product_No"].ToString(),
                            Product_Name = dr["Product_Name"].ToString(),
                            Product_Content = dr["Product_Content"].ToString(),
                            Price = Convert.ToInt32(dr["Price"]),
                            Quantity = Convert.ToInt32(dr["Quantity"]),
                            Image = dr["Image"].ToString(),
                            Recommend = dr["Recommend"].ToString(),
                        };
                        ProductList.Add(product);
                    }
                }
            }
            catch (Exception e)
            {
                ProductList = null;
            }
            finally
            {
                conn.Close();
            }
            return ProductList;
        }
        #endregion

        #region 取得主頁推薦商品
        public List<Products> GetRecommendProducts()
        {
            List<Products> RecommendProductList = new List<Products>();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM Products WHERE Recommend = '1'";
                Sql_cmd.CommandText = sql;

                SqlDataReader dr = Sql_cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Products product = new Products
                        {
                            Product_Id = Convert.ToInt32(dr["Product_Id"]),
                            Product_No = dr["Product_No"].ToString(),
                            Product_Name = dr["Product_Name"].ToString(),
                            Product_Content = dr["Product_Content"].ToString(),
                            Price = Convert.ToInt32(dr["Price"]),
                            Quantity = Convert.ToInt32(dr["Quantity"]),
                            Image = dr["Image"].ToString() == null ? "" : dr["Image"].ToString(),
                            Recommend = dr["Recommend"].ToString(),
                        };
                        RecommendProductList.Add(product);
                    }
                }

            }
            catch(Exception e)
            {
                RecommendProductList = null;
            }
            finally
            {
                conn.Close();
            }
            return RecommendProductList;
        }
        #endregion


    }
}