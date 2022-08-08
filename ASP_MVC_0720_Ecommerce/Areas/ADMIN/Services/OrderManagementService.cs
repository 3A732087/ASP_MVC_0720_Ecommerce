using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using ASP_MVC_0720_Ecommerce.Areas.ADMIN.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services
{
    public class OrderManagementService
    {
        //連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得全部訂單資料
        public List<AdminOrders> GetAllOrderList()
        {
            string sql = @"SELECT * FROM Orders ORDER BY Date DESC";

            List<AdminOrders> OrderList = new List<AdminOrders>();

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;
                Sql_cmd.Parameters.Clear();

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AdminOrders Data = new AdminOrders();
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

            }catch(Exception e)
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

        #region 根據編號取得訂單明細資料
        public List<AdminOrderDetail> GetOrderDetail(string Order_No)
        {

            List<AdminOrderDetail> DetailData = new List<AdminOrderDetail>();
            //AdminOrders OrderData = new AdminOrders();
            string sql = @"SELECT M.Order_No, M.Account, M.Receiver, M.Email, M.Address, M.Date, M.Total, D.OrderDetail_Id, D.Product_No, D.Product_Name, D.Price, D.Qty FROM Orders M
                           INNER JOIN OrderDetail D ON D.Order_No = M.Order_No
                           WHERE M.Order_No = @Order_No
                           ORDER BY Date DESC ";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Order_No", SqlDbType.VarChar).Value = Order_No;


                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        AdminOrderDetail Data = new AdminOrderDetail();
                        Data.OrderDetail_Id = Convert.ToInt32(dr["OrderDetail_Id"]);
                        Data.Account = dr["Account"].ToString();
                        Data.Order_No = dr["Order_No"].ToString();
                        Data.Receiver = dr["Receiver"].ToString();
                        Data.Email = dr["Email"].ToString();
                        Data.Address = dr["Address"].ToString();
                        Data.Date = Convert.ToDateTime(dr["Date"]);
                        Data.Total = Convert.ToInt32(dr["Total"]);

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

        #region 刪除某筆訂單資料
        public void DelOrder(string Order_No)
        {
            string sql = @"DELETE FROM OrderDetail WHERE Order_No = @Order_No;
                           DELETE FROM Orders WHERE Order_No = @Order_No";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Order_No", SqlDbType.VarChar).Value = Order_No;

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

        #region 更新訂單資料(編輯)
        public void EditOrder(AdminOrderDetailViewModel EditOrder)
        {
            string sql_main = @"UPDATE Orders SET Total = @Total WHERE Order_No = @Order_No";
            string sql_detail = @"UPDATE OrderDetail SET Qty = @Qty WHERE OrderDetail_Id = @OrderDetail_Id";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql_main;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("Total", SqlDbType.Int).Value = EditOrder.DetailData[0].Total;
                Sql_cmd.Parameters.Add("Order_No", SqlDbType.NVarChar).Value = EditOrder.DetailData[0].Order_No;
                Sql_cmd.ExecuteNonQuery();


                foreach(var Data in EditOrder.DetailData)
                {
                    Sql_cmd.CommandText = sql_detail;
                    Sql_cmd.Parameters.Clear();
                    Sql_cmd.Parameters.Add("@Qty", SqlDbType.Int).Value = Data.Qty;
                    Sql_cmd.Parameters.Add("@OrderDetail_Id", SqlDbType.Int).Value = Data.OrderDetail_Id;

                    Sql_cmd.ExecuteNonQuery();
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
        }
        #endregion
    }
}