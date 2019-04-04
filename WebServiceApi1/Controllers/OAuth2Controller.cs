using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Flurl.Http;
using WebServiceApi1.Models;

namespace WebServiceApi1.Controllers
{
    /// <summary>
    /// 客户端模式【Client Credentials Grant】
    /// </summary>
    [RoutePrefix("api/v1/oauth2")]
    public class OAuth2Controller : ApiController
    {
        /// <summary>
        /// 获得资讯
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("news")]
        public async Task<IHttpActionResult> GetNewsAsync()
        {
            var authentication = HttpContext.Current.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Bearer").Result;

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var data = claimsIdentity.Claims.Where(c => c.Type == "urn:oauth:scope").ToList();
            var claims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;
            return Ok(new { IsError = true, Msg = string.Empty, Data = Thread.CurrentPrincipal.Identity.Name + " It's about news !!! token expires: " });

            
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        [Route("token")]
        public async Task<IHttpActionResult> GetTokenAsync()
        {
            //获得token
            var dict = new SortedDictionary<string, string>();
            dict.Add("client_id", "aaa");
            dict.Add("client_secret", "123456");
            dict.Add("grant_type", "client_credentials");
            var data = await (@"http://" + Request.RequestUri.Authority + @"/token").PostUrlEncodedAsync(dict).ReceiveJson<Token>();
            //根据token获得咨询信息 [Authorization: Bearer {THE TOKEN}]
            //var news = await (@"http://" + Request.RequestUri.Authority + @"/api/v1/oauth2/news").WithHeader("Authorization", "Bearer " + data.access_token).GetAsync().ReceiveString();
            //var news = await (@"http://" + Request.RequestUri.Authority + @"/api/v1/oauth2/news").WithOAuthBearerToken(data.access_token).GetAsync().ReceiveString();
            return Ok(new { IsError = true, Msg = data, Data = data.access_token });
        }

        public async Task<IHttpActionResult> GetAccessTokenByRefreshTokenTest(string refresh_token)
        {
            //var clientId = "[clientId]";
            //var clientSecret = "[clientSecret]";
            //获得token
            var dict = new SortedDictionary<string, string>();
           
            dict.Add("grant_type", "refresh_token");
            dict.Add("refresh_token", refresh_token);

            var data = await (@"http://" + Request.RequestUri.Authority + @"/token").PostUrlEncodedAsync(dict).ReceiveString();
            return Ok(data);
        }
        
    }
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
