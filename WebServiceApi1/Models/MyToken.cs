using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebServiceApi1.Models
{
    public static class TokenInfo
    {
        //客户端模式的刷新
        //private static Timer clientRefreshTimer = new Timer(async x => await refresh(), null, Infrastructure.REFREST_INTERVAL, Infrastructure.REFREST_INTERVAL);
        //授权码模式的刷新
        private static Timer authorRefreshTimer = new Timer(async x => await clientRefresh(), null, Infrastructure.REFREST_INTERVAL, Infrastructure.REFREST_INTERVAL);
        private static string access_token { get; set; }
        private static string refresh_token { get; set; }
        private static string token_type { get; set; }
        private static string expires_in { get; set; }
        private static string authorization_code { get; set; }
        private static async Task clientRefresh()
        {
            await getToken(Grant_type.Client);
        }
        public static async Task authorRefresh()
        {
            await getToken(Grant_type.Refresh, refresh_token);
        }
        public static async Task<string> getClientToken()
        {
            if (string.IsNullOrEmpty(access_token))
            {
                await clientRefresh();
            }
            return access_token;
        }
        public static string getAuthorToken()
        {
            return access_token;
        }
        public static async Task<string> giveAuthorCode()
        {
            await getAuthorizationCode();
            await getToken(Grant_type.AuthorCode, authorizationCode: authorization_code);
            return access_token;
        }
      
        private static async Task getClientToken(string grant_type, string refreshToken = null)
        {
            HttpClient client = new HttpClient();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grant_type);
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(Infrastructure.CLIENT_ID + ":" + Infrastructure.CLIENT_SECRET)));
            HttpContent content = new FormUrlEncodedContent(parameters);
            content.Headers.Add("data-type", "application/json");

            var result = await client.PostAsync(Infrastructure.API + "/token", content);
            if (result.StatusCode!=HttpStatusCode.OK)
            {
                var error = await result.Content.ReadAsAsync<HttpError>();
                return;
            }
            var token = await result.Content.ReadAsAsync<MyToken>();
            setToken(token.access_token, token.refresh_token, token.token_type, token.expires_in);
        }
        private static void setToken(string access, string refresh, string type, string expires)
        {
            access_token = access;
            refresh_token = refresh;
            token_type = type;
            expires_in = expires;
        }
        /// <summary>
        /// 获取accesstoken（授权码模式，客户端模式，密码模糊？）
        /// </summary>
        /// <param name="grantType"></param>
        /// <param name="refreshToken"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        private static async Task getToken(string grantType, string refreshToken = null, string userName = null, string password = null, string authorizationCode = null)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress= new Uri(Infrastructure.API);
            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", grantType);

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                parameters.Add("username", userName);
                parameters.Add("password", password);
            }
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                parameters.Add("code", authorizationCode);
                parameters.Add("redirect_uri", $"{Infrastructure.API}/api/authorization_code"); //和获取 authorization_code 的 redirect_uri 必须一致，不然会报错
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                parameters.Add("refresh_token", refreshToken);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(Infrastructure.CLIENT_ID + ":" + Infrastructure.CLIENT_SECRET)));

            var response = await client.PostAsync("/token", new FormUrlEncodedContent(parameters));
            var responseValue = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = await response.Content.ReadAsAsync<HttpError>();
                return;
            }
            var token = await response.Content.ReadAsAsync<MyToken>();
            setToken(token.access_token, token.refresh_token, token.token_type, token.expires_in);
        }
        /// <summary>
        /// 授权码（授权码模式）
        /// </summary>
        /// <returns></returns>
        private  static async Task getAuthorizationCode()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Infrastructure.API);

            var response = await client.GetAsync($"/authorize?grant_type=authorization_code&response_type=code&client_id={Infrastructure.CLIENT_ID}&client_secret={Infrastructure.CLIENT_SECRET}&redirect_uri={HttpUtility.UrlEncode($"{Infrastructure.API}/api/authorization_code")}");
            var code = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = await response.Content.ReadAsAsync<HttpError>();
            }
            authorization_code = code;
        }
        public static async Task<string> getImplicitToken()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Infrastructure.API);
            var tokenResponse = await client.GetAsync($"/authorize?response_type=token&client_id={Infrastructure.CLIENT_ID}&client_secret={Infrastructure.CLIENT_SECRET}&redirect_uri={HttpUtility.UrlEncode($"{Infrastructure.API}/api/access_token")}");
            //redirect_uri: http://localhost:8001/api/access_token#access_token=AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAfoPB4HZ0PUe-X6h0UUs2q42&token_type=bearer&expires_in=10
            var responseValue = tokenResponse.RequestMessage.RequestUri.Fragment.Split('&');//get form redirect_uri
            access_token = responseValue[0].Split('=')[1];
            token_type = responseValue[1].Split('=')[1];
            expires_in = responseValue[2].Split('=')[1];
            return access_token;
        }
    }
    public static class Grant_type
    {
        public const string Client = "client_credentials";
        public const string Refresh = "refresh_token";
        public const string AuthorCode = "authorization_code";
        public const string Password = "";
    }
    public static class Infrastructure
    {
        public static readonly string API = "http://localhost:8001";
        public static readonly string CLIENT_ID = "aaa";
        public static readonly string CLIENT_SECRET = "123456";
        public static readonly string GRANT_TYPE_GET = "client_credentials";
        public static readonly string GRANT_TYPE_REFRESH = "refresh_token";
        public static readonly int REFREST_INTERVAL = 600000;
    }
    public class MyToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    public class HttpError
    {
        public string error { get; set; }
        public string errorDescription { get; set; }
    }
}