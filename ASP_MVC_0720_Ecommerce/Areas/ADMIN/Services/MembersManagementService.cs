using ASP_MVC_0720_Ecommerce.Areas.ADMIN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Areas.ADMIN.Services
{
    public class MembersManagementService
    {
        //連線字串
        private static readonly string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;

        //資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 取得所有會員資料
        public List<AdminMembers> GetAllMembers()
        {
            string sql = @"SELECT * FROM Members ";
            List<AdminMembers> DataList = new List<AdminMembers>();
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
                        AdminMembers Data = new AdminMembers();
                        Data.Account = dr["Account"].ToString();
                        Data.Password = dr["Password"].ToString();
                        Data.Name = dr["Name"].ToString();
                        Data.Email = dr["Email"].ToString();
                        Data.AuthCode = dr["AuthCode"].ToString();
                        Data.IsAdmin = dr["IsAdmin"].ToString();
                        Data.LineNotifyAccessToken = dr["LineNotifyAccessToken"].ToString();
                        DataList.Add(Data);
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
            return DataList;
        }
        #endregion

        #region 取得某筆會員資料
        public AdminMembers GetMemberByAccount(string Account)
        {
            string sql = @"SELECT * FROM Members WHERE Account = @Account";
            AdminMembers member = new AdminMembers();
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    member.Account = dr["Account"].ToString();
                    member.Password = dr["Password"].ToString();
                    member.Name = dr["Name"].ToString();
                    member.Email = dr["Email"].ToString();
                    member.AuthCode = dr["AuthCode"].ToString();
                    member.IsAdmin = dr["IsAdmin"].ToString();
                    member.LineNotifyAccessToken = dr["LineNotifyAccessToken"].ToString();
                }
            }catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return member;
        }
        #endregion

        #region 刪除會員資料
        public void DeleteMember(string Account)
        {
            string sql = @"DELETE FROM Members WHERE Account = @Account";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;

                Sql_cmd.CommandText = sql;
                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                Sql_cmd.ExecuteNonQuery();

            }catch(Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region Hash密碼
        public string HashPassword(string Password)
        {
            //hash加鹽
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            //密碼與加鹽結合
            string saltAndPassword = string.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //密碼轉為byte
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //Hash byte 資料
            byte[] HashData = sha256Hasher.ComputeHash(PasswordData);
            //Hash後資料轉為string
            string HashResult = Convert.ToBase64String(HashData);

            return HashResult;
        }
        #endregion

        #region 編輯會員資料(Update)
        public void EditMember(AdminMembers editMemberData)
        {
            string sql = @"UPDATE Members SET Password = @Password , Name = @Name, Email = @Email, AuthCode = @AuthCode , IsAdmin = @IsAdmin , LineNotifyAccessToken = @LineNotifyAccessToken WHERE Account = @Account";

            editMemberData.Password = HashPassword(editMemberData.Password);
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;

                if (editMemberData.AuthCode == "          ")
                {
                    editMemberData.AuthCode = String.Empty;
                }



                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = editMemberData.Password;
                Sql_cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = editMemberData.Name;
                Sql_cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = editMemberData.Email;
                Sql_cmd.Parameters.Add("@AuthCode", SqlDbType.NChar).Value = editMemberData.AuthCode;
                Sql_cmd.Parameters.Add("@IsAdmin", SqlDbType.NVarChar).Value = editMemberData.IsAdmin;
                if (string.IsNullOrEmpty(editMemberData.LineNotifyAccessToken))
                {
                    Sql_cmd.Parameters.Add("@LineNotifyAccessToken", SqlDbType.NVarChar).Value = DBNull.Value;
                }
                else
                {
                    Sql_cmd.Parameters.Add("@LineNotifyAccessToken", SqlDbType.NVarChar).Value = editMemberData.LineNotifyAccessToken;
                }
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = editMemberData.Account;

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