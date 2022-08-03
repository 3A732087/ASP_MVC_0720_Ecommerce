using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using ASP_MVC_0720_Ecommerce.Security;
using ASP_MVC_0720_Ecommerce.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersDBService membersService = new MembersDBService();
        private readonly MailService mailService = new MailService();

        #region Line Notify參數
        string _client_id = WebConfigurationManager.AppSettings["client_id"].ToString();
        string _client_secret = WebConfigurationManager.AppSettings["client_secret"].ToString();
        string _redirect_uri = WebConfigurationManager.AppSettings["redirect_uri"].ToString();
        #endregion

        #region 登入
        //登入畫面
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "HomeWeb", new { area = "SHOP" });
            return View();
        }

        //傳入登入資料Action
        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            if (String.IsNullOrEmpty(ValidateStr))
            {
                HttpContext.Session.Clear();
                //設定登入者名稱
                Session["Username"] = membersService.GetDatabyAccount(LoginMember.Account).Name.ToString();
                //藉由Service取得登入角色
                string RoleData = membersService.GetRole(LoginMember.Account);
                //設定JWT
                JwtService jwtService = new JwtService();
                //從web.config撈出
                //cookie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                //產生一個cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設訂單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定Cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                //重新導向
                return RedirectToAction("Index", "HomeWeb", new { area = "SHOP" });
            }
            else
            {
                //有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("LoginError", ValidateStr);
                return View(LoginMember);
            }
        }
        #endregion

        #region 註冊
        //註冊顯示頁面
        public ActionResult Register()
        {
            if(User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "HomeWeb",new { area = "SHOP"});
            return View();
        }

        [HttpPost]
        //傳入註冊資料Action
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷頁面資料是否都經過驗證(Model)
            if (ModelState.IsValid)
            {
                //將頁面資料中的密碼填入
                RegisterMember.newMember.Password = RegisterMember.Password;
                //取得信箱驗證碼
                string AuthCode = mailService.GetValidateCode();
                //將信箱消驗證碼填入
                RegisterMember.newMember.AuthCode = AuthCode;
                //註冊會員
                membersService.Register(RegisterMember.newMember);
                //信件範本
                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                //宣告Email驗證用的Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members"
                    , new
                    {
                        Account = RegisterMember.newMember.Account,
                        AuthCode = AuthCode
                    })
                };

                string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));

                //呼叫Service寄出驗證信
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);
                //用TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";

                return RedirectToAction("RegisterResult");
            }
            //未經驗證清除密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;

            return View(RegisterMember);
        }

        //註冊結果顯示
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷帳號是否註冊過
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來的Action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }
        #endregion

        #region 登出
        [Authorize] //設定此Action需登入
        public ActionResult Logout()
        {
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //清除session
            Session.Remove("Username");
            //重新導向至登入Action
            return RedirectToAction("Index", "HomeWeb", new { area = "SHOP" });
        }
        #endregion

        #region 修改密碼
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }


        //修改密碼傳入資料
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            if (ModelState.IsValid)
            {
                TempData["msg"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion

        #region Line Notify設定
        //設定畫面
        public ActionResult LineNotifySetting()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Members", new { area = "SHOP" });

            ViewBag.AccessToken = membersService.GetLineNotifyAccessToken(User.Identity.Name);
            return View();
        }

        //連動按鈕
        public ActionResult GetLineNotifyLoginUrl()
        {
            if(Request.IsAjaxRequest() == false)
            {
                return Content("");
            }
            string state = Guid.NewGuid().ToString();
            TempData["state"] = state;
            string LineLoginUrl = $@"https://notify-bot.line.me/oauth/authorize?response_type=code&client_id={_client_id}&redirect_uri={_redirect_uri}&scope=notify&state={state}";            

            return Content(LineLoginUrl);
        }

        //取得AccessToken
        [Authorize]
        public ActionResult AfterLineNotifyLogin(string state, string code, string error, string error_description)
        {
            if (!string.IsNullOrEmpty(error))
            {//用戶沒授權你的LineApp
                ViewBag.error = error;
                ViewBag.error_description = error_description;
                return View();
            }

            if (TempData["state"] == null)
            {//可能使用者停留Line登入頁面太久
                return Content("頁面逾期");
            }

            if (Convert.ToString(TempData["state"]) != state)
            {//使用者原先Request QueryString的TempData["state"]和Line導頁回來夾帶的state Querystring不一樣，可能是parameter tampering或CSRF攻擊
                return Content("state驗證失敗");
            }

            if (Convert.ToString(TempData["state"]) == state)
            {
                //state字串驗證通過
                Dictionary<string, object> l_result = new Dictionary<string, object>();

                string Url = "https://notify-bot.line.me/oauth/token";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                request.Method = "POST";
                request.KeepAlive = true; //是否保持連線
                request.ContentType = "application/x-www-form-urlencoded";
                string posturi = "";
                posturi += "grant_type=authorization_code";
                posturi += "&code=" + code; //Authorize code
                posturi += "&redirect_uri=" + _redirect_uri;
                posturi += "&client_id=" + _client_id;
                posturi += "&client_secret=" + _client_secret;

                byte[] bytes = Encoding.UTF8.GetBytes(posturi);//轉byte[]

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();//回傳JSON
                responseString = "[" + responseString + "]";
                response.Close();

                var token = JsonConvert.DeserializeObject<JArray>(responseString)[0]["access_token"].ToString();
                var Status = JsonConvert.DeserializeObject<JArray>(responseString)[0]["status"].ToString();

                ViewBag.access_token = token;
                ViewBag.status = Status;

                //寫入Sql 
                membersService.UpdateLineNotifyAccessToken(User.Identity.Name, token);
            }
            return View();
        }

        //取消綁定LineNotify
        [Authorize]
        public ActionResult RevokeLineNotify()
        {
            string AccessToken = membersService.GetLineNotifyAccessToken(User.Identity.Name);

            string Url = "https://notify-api.line.me/api/revoke";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "POST";
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var response = request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();//回傳JSON
            responseString = "[" + responseString + "]";
            response.Close();

            var Status = JsonConvert.DeserializeObject<JArray>(responseString)[0]["status"].ToString();

            membersService.UpdateLineNotifyAccessToken(User.Identity.Name, "");
            TempData["msg"] = "連動已解除。";

            return RedirectToAction("LineNotifySetting");
        }
        #endregion
    }
}