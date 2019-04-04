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
    public class MyRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> _refreshToken = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 生成 refresh_token
        /// </summary>
        /// <param name="context"></param>
        public void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            context.SetToken(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            _refreshToken[context.Token] = context.SerializeTicket();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            context.SetToken(Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            _refreshToken[context.Token] = context.SerializeTicket();
            
        }
        /// <summary>
        /// 由 refresh_token 解析成 access_token
        /// </summary>
        /// <param name="context"></param>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_refreshToken.TryRemove(context.Token,out value))
            {
                context.DeserializeTicket(value);
            }
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_refreshToken.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
            return Task.FromResult<object>(null);


        }
    }
}