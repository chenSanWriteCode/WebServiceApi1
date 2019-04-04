using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebServiceApi1.Models;
using WebServiceApi1.Provider;
namespace WebServiceApi1
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //授权码模式
            var authOption = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                AuthenticationMode = AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/token"), //获取 access_token 授权服务请求地址
                AuthorizeEndpointPath = new PathString("/authorize"), //获取 authorization_code 授权服务请求地址
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(1), //access_token 过期时间

                Provider = new MyOAuthAuthorizationServerProvider(), //access_token 相关授权服务
                AuthorizationCodeProvider = new MyAuthAuthorizationCodeProvider(), //authorization_code 授权服务
                //可以提供也可不提供，如果想用自己的规则就可以实现AccessTokenProvider
                //AccessTokenProvider= new MyAuthAuthorizationCodeProvider(),
                RefreshTokenProvider = new MyRefreshTokenProvider()//refresh_token 授权服务
            };
            //简化模式(授权码模式简化版本)
            var implicitOption = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                AuthenticationMode = AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/token"), //获取 access_token 授权服务请求地址
                AuthorizeEndpointPath = new PathString("/authorize"),//获取 authorization_code 授权服务请求地址
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(Infrastructure.REFREST_INTERVAL), //access_token 过期时间

                Provider = new MyOAuthAuthorizationServerProvider(), //access_token 相关授权服务
                AuthorizationCodeProvider = new MyAuthAuthorizationCodeProvider(), //authorization_code 授权服务

                //不支持用refreshtoken刷新accesstoken
                //RefreshTokenProvider= new MyRefreshTokenProvider()
            };

            //客户端模式
            var clientOption = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                AuthenticationMode = AuthenticationMode.Active,
                TokenEndpointPath = new PathString("/token"), //获取 access_token 授权服务请求地址
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(Infrastructure.REFREST_INTERVAL), //access_token 过期时间

                Provider = new MyOAuthClientServerProvider(), //access_token 相关授权服务

                //可以提供也可不提供，可以在provider中默认生成access_token,如果想用自己的规则就可以实现AccessTokenProvider
                //AccessTokenProvider= new MyAuthAuthorizationCodeProvider(),

                //不需要实现，因为客户端模式不支持用refreshtoken刷新accesstoken，没有必要
                //RefreshTokenProvider= new MyRefreshTokenProvider()
            };


            //The UseOAuthBearerTokens extension method creates both the token server and the middleware to validate tokens for requests in the same application.
            app.UseOAuthBearerTokens(authOption);
            //app.UseOAuthBearerTokens(implicitOption);
            //app.UseOAuthBearerTokens(clientOption);

            // application bearer token middleware   
            //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions { });

            // authorization server middleware
           // app.UseOAuthAuthorizationServer(authOption);


        }




    }
}