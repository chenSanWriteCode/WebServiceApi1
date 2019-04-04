using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using WebServiceApi1.Models;

namespace WebServiceApi1.Provider
{
    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 验证客户端
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            if (context.ClientId == null)
            {
                context.SetError("invalid clientId", "clientID is null");
                return Task.FromResult<object>(null);
            }
            var client = ClientRepository.Clients.Where(x => x.id == clientId && x.secret==clientSecret).FirstOrDefault();
            if (client != null)
            {
                context.Validated(clientId);
            }
            else
            {
                context.SetError("Invalid ClientID", $"error clientID {clientId}");
                return Task.FromResult<object>(null);
            }
            return Task.FromResult<object>(null);
        }
        /// <summary>
        /// 生成授权码 
        /// 生成 authorization_code（authorization code 授权方式）、生成 access_token （implicit 授权模式）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            if (context.AuthorizeRequest.IsImplicitGrantType)
            {
                //implicit 授权方式(简化模式)
                var identity = new ClaimsIdentity("Bearer");
                context.OwinContext.Authentication.SignIn(identity);
                context.RequestCompleted();
            }
            else if (context.AuthorizeRequest.IsAuthorizationCodeGrantType)
            {
                //authorization code 授权方式(授权码)
                var redirectUri = context.Request.Query["redirect_uri"];
                var clientId = context.Request.Query["client_id"];
                var identity = new ClaimsIdentity(new GenericIdentity(clientId, OAuthDefaults.AuthenticationType));

                var authorizeCodeContext = new AuthenticationTokenCreateContext(
                    context.OwinContext,
                    context.Options.AuthorizationCodeFormat,
                    new AuthenticationTicket(
                    identity,
                    new AuthenticationProperties(new Dictionary<string, string>{
                    {"client_id", clientId},
                        {"redirect_uri", redirectUri}
                    })
                    {
                        IssuedUtc = DateTimeOffset.UtcNow,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(context.Options.AuthorizationCodeExpireTimeSpan)
                    }));
                await context.Options.AuthorizationCodeProvider.CreateAsync(authorizeCodeContext);
                context.Response.Redirect(redirectUri + "?Code=" + Uri.EscapeDataString(authorizeCodeContext.Token));
                context.RequestCompleted();
            }
        }
        /// <summary>
        /// 验证 authorization_code 的请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            var client = ClientRepository.Clients.Where(x => x.id == context.AuthorizeRequest.ClientId).FirstOrDefault();
            //授权方式为 授权码或者简化模式
            if (client!=null&&(context.AuthorizeRequest.IsAuthorizationCodeGrantType||context.AuthorizeRequest.IsImplicitGrantType))
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }
        /// <summary>
        /// 验证 redirect_uri
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            var client = ClientRepository.Clients.Where(x => x.id == context.ClientId).FirstOrDefault();
            if (client != null)
            {
                var expectedRootUri = new Uri(context.Request.Uri, "/");
                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
                //context.Validated(client.redirectUrl);
                context.Validated(context.RedirectUri);
            }
        }
        /// <summary>
        /// 验证 access_token 的请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {

            if (context.TokenRequest.IsAuthorizationCodeGrantType || context.TokenRequest.IsRefreshTokenGrantType)
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            if (context.Ticket==null ||context.Ticket.Identity==null ||!context.Ticket.Identity.IsAuthenticated)
            {
                context.SetError("invalid_grant", "Refresh token is not valid");
            }
            else
            {
                //Additional claim is needed to separate access token updating from authentication 
                //requests in RefreshTokenProvider.CreateAsync() method
            }
            return base.GrantRefreshToken(context);
        }
    }
}