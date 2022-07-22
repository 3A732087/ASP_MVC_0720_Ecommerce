﻿using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ASP_MVC_0720_Ecommerce.Security
{
    public class JwtService
    {
        #region 製作Token
        public string GenerateToken(string Account, string Role)
        {
            JwtObject jwtObject = new JwtObject
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };

            //從Web.Config取得密鑰
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            var payload = jwtObject;
            //將資料加密為Token
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
            return token;
        }
        #endregion
    }
}
