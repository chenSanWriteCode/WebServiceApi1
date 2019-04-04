using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceApi1.Provider
{
    public class Client
    {
        public string id { get; set; }
        public string secret { get; set; }
        public string redirectUrl { get; set; }
    }
    public class ClientRepository
    {
        public static List<Client> Clients = new List<Client>()
        {
            new Client {id="aaa",secret="123456",redirectUrl="http://localhost:6745" },
            new Client{id="bbb",secret="234567",redirectUrl="http://localhost:6745" }
        };
    }
}