using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;
using WebServiceApi1.Models;

namespace WebServiceApi1.Provider
{
    public class MyAuthAuthorizationCodeProvider : IAuthenticationTokenProvider
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        /// <summary>
        /// 生成 authorization_code
        /// </summary>
        /// <param name="context"></param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            context.SetToken(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }
        /// <summary>
        /// anthorize_code
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);
            context.SetToken(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }
        /// <summary>
        /// 由 authorization_code 解析成 access_token
        /// </summary>
        /// <param name="context"></param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token,out value))
            {
                context.DeserializeTicket(value);
            }
        }
        /// <summary>
        /// access_token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
    }
}