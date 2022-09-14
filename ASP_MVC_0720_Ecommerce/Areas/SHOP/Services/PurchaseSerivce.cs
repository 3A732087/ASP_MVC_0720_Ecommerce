using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Services
{
    public class PurchaseSerivce
    {
        //資料庫連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 結帳
        public Dictionary<string, object> CheckoutAll(CheckoutViewModel newCheckout)
        {
            conn.Open();
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            SqlCommand Sql_cmd = new SqlCommand();
            Sql_cmd.Connection = conn;
            Sql_cmd.Transaction = trans;
            string sql;

            Dictionary<string, object> _result = new Dictionary<string, object>();
            try
            {
                #region 訂單主檔寫入
                sql = @"INSERT INTO Orders (Account, Receiver, Email, Address, Total, Date) VALUES (@Account, @Receiver, @Email, @Address, @Total, @Date);SELECT CONVERT(INT, SCOPE_IDENTITY());";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                //Sql_cmd.Parameters.Add("@Order_No", SqlDbType.NVarChar).Value = "test";
                Sql_cmd.Parameters.Add("@Account", SqlDbType.NVarChar).Value = newCheckout.newOrder.Account;
                Sql_cmd.Parameters.Add("@Receiver", SqlDbType.NVarChar).Value = newCheckout.newOrder.Receiver;
                Sql_cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = newCheckout.newOrder.Email;
                Sql_cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = newCheckout.newOrder.Address;
                Sql_cmd.Parameters.Add("@Total", SqlDbType.Int).Value = newCheckout.newOrder.Total;
                Sql_cmd.Parameters.Add("@Date", SqlDbType.NVarChar).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                int pkid = (Int32)Sql_cmd.ExecuteScalar();
                #endregion

                #region 訂單編號寫入
                sql = @"UPDATE Orders　SET Order_No = @Order_No WHERE Order_Id = @Order_Id";

                string Order_No = "T" + DateTime.Now.ToString("yyyyMMdd") + string.Format("{0:0000}", Convert.ToInt32(pkid));

                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Order_No", SqlDbType.NVarChar).Value = Order_No;
                Sql_cmd.Parameters.Add("@Order_Id", SqlDbType.Int).Value = pkid;

                Sql_cmd.ExecuteNonQuery();
                #endregion

                #region 訂單明細寫入
                foreach (Carts product in newCheckout.Product)
                {
                    sql = @"INSERT INTO OrderDetail (Order_No, Account, Product_No, Product_Name, Price, Qty) VALUES (@Order_No, @Account, @Product_No, @Product_Name, @Price, @Qty)";
                    Sql_cmd.CommandText = sql;

                    Sql_cmd.Parameters.Clear();
                    Sql_cmd.Parameters.Add("@Order_No", SqlDbType.NVarChar).Value = Order_No;
                    Sql_cmd.Parameters.Add("@Account", SqlDbType.NVarChar).Value = newCheckout.newOrder.Account;
                    Sql_cmd.Parameters.Add("@Product_No", SqlDbType.NVarChar).Value = product.Product.Product_No;
                    Sql_cmd.Parameters.Add("@Product_Name", SqlDbType.VarChar).Value = product.Product.Product_Name;
                    Sql_cmd.Parameters.Add("@Price", SqlDbType.NVarChar).Value = product.Product.Price;
                    Sql_cmd.Parameters.Add("@Qty", SqlDbType.Int).Value = product.Qty;

                    Sql_cmd.ExecuteNonQuery();


                    #endregion

                    #region 更新商品數量
                    sql = @"  UPDATE Products set Quantity  = Quantity - @Qty where 1=1 and Product_No = @Product_No";
                    Sql_cmd.CommandText = sql;
                    Sql_cmd.Parameters.Clear();
                    Sql_cmd.Parameters.Add("@Qty", SqlDbType.Int).Value = product.Qty;
                    Sql_cmd.Parameters.Add("@Product_No", SqlDbType.NVarChar).Value = product.Product.Product_No;
                    Sql_cmd.ExecuteNonQuery();
                    #endregion
                }

                #region 清除購物車資料
                sql = @"DELETE FROM Carts WHERE Account = @Account";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.NVarChar).Value = newCheckout.newOrder.Account;

                Sql_cmd.ExecuteNonQuery();
                #endregion

                trans.Commit();

                _result.Add("Result", true);
                _result.Add("Order_No", Order_No);
                _result.Add("Msg", "結帳完成");
            }
            catch (Exception e)
            {
                _result.Add("Result", false);
                _result.Add("Order_No", "");
                _result.Add("Msg", e.Message.ToString());
                trans.Rollback();
                throw new Exception(e.Message.ToString());

            }
            finally
            {
                conn.Close();
                trans.Dispose();
            }
            return _result;
        }
        #endregion

    }
}