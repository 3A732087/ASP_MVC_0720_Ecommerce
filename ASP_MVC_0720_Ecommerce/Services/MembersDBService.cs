using ASP_MVC_0720_Ecommerce.Areas.SHOP.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ASP_MVC_0720_Ecommerce.Services
{
    public class MembersDBService
    {
        //建立與資料庫連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP_NET_0720_Ecommerce"].ConnectionString;
        //建立資料庫連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 註冊
        public void Register(Members newMember)
        {
            //Hash密碼
            newMember.Password = HashPassword(newMember.Password);

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = conn.CreateCommand();

                //Sql寫入
                string sql = @"INSERT INTO Members (Account, Password, Name, Email, AuthCode, IsAdmin) VALUES (@Account, @Password, @Name, @Email, @AuthCode, @IsAdmin)";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = newMember.Account;
                Sql_cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = newMember.Password;
                Sql_cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = newMember.Name;
                Sql_cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = newMember.Email;
                Sql_cmd.Parameters.Add("@AuthCode", SqlDbType.NChar).Value = newMember.AuthCode;
                Sql_cmd.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = false;

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

        #region 查詢一筆資料(private)
        //藉由帳號查詢單筆資料
        //因會員資料隱密性，所以用private限制只能在Service內使用
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = conn.CreateCommand();

                string sql = @"select * from Members where 1=1 and Account = @Account";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
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

        #region 帳號註冊重複確認
        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            bool result = (Data == null);
            return result;
        }
        #endregion

        #region 查詢一筆公開性資料
        //藉由帳號查詢單筆資料
        public Members GetDatabyAccount(string Account)
        {
            Members Data = new Members();
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = conn.CreateCommand();

                string sql = @"select * from Members where 1=1 and Account = @Account";
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
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

        #region 信箱驗證
        public string EmailValidate(string Account, string AuthCode)
        {
            //取得傳入帳號的資料
            Members ValidateMember = GetDataByAccount(Account);
            //宣告驗證後訊息字串
            string ValidateStr = string.Empty;
            if(ValidateMember != null)
            {
                //判斷傳入驗證碼與資料庫中是否相同
                if(ValidateMember.AuthCode == AuthCode)
                {
                    try
                    {
                        conn.Open();
                        SqlCommand Sql_cmd = conn.CreateCommand();

                        string sql = @"UPDATE MEMBERS SET AuthCode = @AuthCode WHERE Account = @Account";
                        Sql_cmd.CommandText = sql;

                        Sql_cmd.Parameters.Clear();
                        Sql_cmd.Parameters.Add("@AuthCode", SqlDbType.NChar).Value = string.Empty;
                        Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

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
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了。";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊。";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或再註冊。";
            }
            return ValidateStr;
        }
        #endregion

        #region 登入確認
        public string LoginCheck(string Account, string Password)
        {
            Members LoginMember = GetDataByAccount(Account);
            if(LoginMember != null)
            {
                //判斷是否Email驗證
                if (string.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }
        #endregion

        #region 密碼確認
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            return result;
        }
        #endregion

        #region 取得角色權限
        public string GetRole(string Account)
        {
            string Role = "User";
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";
            }
            return Role;
        }
        #endregion

        #region 變更密碼
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);

            if(PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);

                string sql = @"UPDATE Members SET Password = @Password WHERE Account = @Account";

                try
                {
                    conn.Open();
                    SqlCommand Sql_cmd = conn.CreateCommand();
                    Sql_cmd.CommandText = sql;

                    Sql_cmd.Parameters.Clear();
                    Sql_cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = LoginMember.Password;
                    Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                    Sql_cmd.ExecuteNonQuery();

                }
                catch(Exception e)
                {
                    throw new Exception(e.Message.ToString());
                }
                finally
                {
                    conn.Close();
                }
                return "修改密碼成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion

        #region 撈取LineNotifyAccessToken
        public string GetLineNotifyAccessToken(string Account)
        {
            string sql = @"SELECT Account, LineNotifyAccessToken FROM Members WHERE Account = @Account";

            string result;
            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

                SqlDataReader dr = Sql_cmd.ExecuteReader();
                dr.Read();
                result = dr["LineNotifyAccessToken"].ToString();
            }
            catch (Exception e)
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

        #region LineNotify寫入(Update)
        public void UpdateLineNotifyAccessToken(string Account, string AccessToken)
        {
            string sql = @"UPDATE Members SET LineNotifyAccessToken = @LineNotifyAccessToken WHERE Account = @Account";

            try
            {
                conn.Open();
                SqlCommand Sql_cmd = new SqlCommand();
                Sql_cmd.Connection = conn;
                Sql_cmd.CommandText = sql;

                Sql_cmd.Parameters.Clear();
                Sql_cmd.Parameters.Add("@LineNotifyAccessToken", SqlDbType.NVarChar).Value = AccessToken;
                Sql_cmd.Parameters.Add("@Account", SqlDbType.VarChar).Value = Account;

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