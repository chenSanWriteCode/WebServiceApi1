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
    /// <summary>
    /// 客户端模式
    /// </summary>
    public class MyOAuthClientServerProvider : OAuthAuthorizationServerProvider
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

            var client = ClientRepository.Clients.Where(x => x.id == clientId && x.secret == clientSecret).FirstOrDefault();
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
        /// 验证 access_token 的请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            if (context.TokenRequest.IsClientCredentialsGrantType)
            {
                context.Validated();
            }
            else
            {
                context.Rejected();
            }
        }
        /// <summary>
        /// 客户端授权[生成access token]
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            //var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "iphone"));
            //var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties() { IssuedUtc = DateTime.UtcNow, ExpiresUtc = DateTime.UtcNow.AddMilliseconds(Infrastructure.REFREST_INTERVAL) });
            //context.Validated(ticket);
            //return base.GrantClientCredentials(context);
            var authidentity = new ClaimsIdentity(new GenericIdentity(context.ClientId, context.Options.AuthenticationType),context.Scope.Select(x=>new Claim("",x)));
            context.Validated(authidentity);
            return Task.FromResult<object>(null);
            
        }


    }
}