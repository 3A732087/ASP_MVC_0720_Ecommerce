using ASP_MVC_0720_Ecommerce.Areas.SHOP.ViewModels;
using ASP_MVC_0720_Ecommerce.Security;
using ASP_MVC_0720_Ecommerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ASP_MVC_0720_Ecommerce.Areas.SHOP.Controllers
{
    public class MembersController : Controller
    {
        private readonly MembersDBService membersService = new MembersDBService();
        private readonly MailService mailService = new MailService();

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
    }
}