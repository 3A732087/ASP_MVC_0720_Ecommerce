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
    public class OrderService
    {
        //連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;

        //連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 取得訂單主檔
        public List<Order> GetOrderList(string Account)
        {
            List<Order> OrderList = new List<Order>();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM Orders WHERE Account = @Account ORDER BY Order_Id DESC";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Order Data = new Order();
                        Data.Order_No = dr["Order_No"].ToString();
                        Data.Account = dr["Account"].ToString();
                        Data.Receiver = dr["Receiver"].ToString();
                        Data.Email = dr["Email"].ToString();
                        Data.Address = dr["Address"].ToString();
                        Data.Date = Convert.ToDateTime(dr["Date"]);
                        Data.Total = Convert.ToInt32(dr["Total"]);
                        OrderList.Add(Data);
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
            return OrderList;
        }
        #endregion

        #region 取得訂單明細
        public List<OrderDetail> GetOrderDetail(string Account, string Order_No)
        {
            List<OrderDetail> DetailData = new List<OrderDetail>();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                string sql = @"SELECT * FROM OrderDetail WHERE Account = @Account AND Order_No = @Order_No";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;
                Sql_cmd.Parameters.Add("@Order_No", SqlDbType.VarChar).Value = Order_No;


                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        OrderDetail Data = new OrderDetail();
                        Data.Order_No = dr["Order_No"].ToString();
                        Data.Account = dr["Account"].ToString();
                        Data.Product_No = dr["Product_No"].ToString();
                        Data.Product_Name = dr["Product_Name"].ToString();
                        Data.Price = Convert.ToInt32(dr["Price"]);
                        Data.Qty = Convert.ToInt32(dr["Qty"]);
                        DetailData.Add(Data);
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
            return DetailData;
        }
        #endregion
    }
}