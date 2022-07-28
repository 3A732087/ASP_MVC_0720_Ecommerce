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
    public class CartsService
    {
        //資料庫連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得購物車內商品
        public List<Carts> GetCartProduct(string Account)
        {
            List<Carts> AllCarts = new List<Carts>();
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM Carts M INNER JOIN Products D ON D.Product_Id = M.Product_Id WHERE 1=1 AND M.Account = @Account";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Carts AllCart = new Carts();
                        AllCart.Cart_Id = Convert.ToInt32(dr["Cart_Id"]);
                        AllCart.Account = dr["Account"].ToString();
                        AllCart.Product_Id = Convert.ToInt32(dr["Product_Id"]);
                        AllCart.Qty = Convert.ToInt32(dr["Qty"]);
                        AllCart.Product.Product_No = dr["Product_No"].ToString();
                        AllCart.Product.Product_Name = dr["Product_Name"].ToString();
                        AllCart.Product.Product_Content = dr["Product_Content"].ToString();
                        AllCart.Product.Price = Convert.ToInt32(dr["Price"]);
                        AllCart.Product.Image = dr["Image"].ToString() == null ? "" : dr["Image"].ToString();
                        AllCart.Product.Recommend = dr["Recommend"].ToString();
                        AllCart.Product.Quantity = Convert.ToInt32(dr["Quantity"]);
                        AllCarts.Add(AllCart);
                    }
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
            return AllCarts;
        }
        #endregion

        #region 加入購物車
        public void AddtoCart(string Account, int Product_Id, int Qty, bool InCart)
        {
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                string sql;
                if (InCart == false)
                    sql = @"INSERT INTO Carts (Account, Product_Id, Qty) VALUES (@Account, @Product_Id, @Qty)";
                else
                    sql = @"UPDATE Carts SET Qty =  @Qty WHERE Account = @Account AND Product_Id = @Product_Id";

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;
                Sql_cmd.Parameters.Add("@Product_Id", SqlDbType.VarChar).Value = Product_Id;
                Sql_cmd.Parameters.Add("@Qty", SqlDbType.VarChar).Value = Qty;

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

        #region 商品是否存在購物車
        public bool CheckInCart(string Account, int Product_Id)
        {
            bool result;
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM Carts WHERE Account = @Account AND Product_Id = @Product_Id";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;
                Sql_cmd.Parameters.Add("@Product_Id", SqlDbType.Int).Value = Product_Id;

                SqlDataReader dr =  Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                    result =  true;
                else
                    result = false;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }

            return result;
        }
        #endregion

        #region 取得商品在購物車內的數量
        public int GetProductInCartQty(string Account, int Product_Id)
        {
            int Qty = 0;
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM Carts WHERE Account = @Account AND Product_Id = @Product_Id";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;
                Sql_cmd.Parameters.Add("@Product_Id", SqlDbType.Int).Value = Product_Id;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    Qty = Convert.ToInt32(dr["Qty"]);
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
            return Qty;
        }
        #endregion

        #region 移除購物車
        public void RemoveFromCart(int Cart_Id)
        {
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                string sql = @"DELETE FROM Carts WHERE Cart_Id = @Cart_Id";

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Cart_Id", SqlDbType.Int).Value = Cart_Id;
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