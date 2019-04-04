using System.Threading.Tasks;
using System.Web.Mvc;
using WebServiceApi1.Models;

namespace WebServiceApi1.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public async Task<string> GetApiData()
        {
            string token = await TokenInfo.getClientToken();
            return token;
        }

        public async Task<string> GiveAuthorCode()
        {
            string code = await TokenInfo.giveAuthorCode();
            return code;
        }
        public async Task<string> getAuthorToken()
        {
            return await Task.Factory.StartNew(() => TokenInfo.getAuthorToken());
        }
        public async Task<string> RefreshAuthorToken()
        {
            await TokenInfo.authorRefresh();
            return TokenInfo.getAuthorToken();
        }
        public async Task<string> ImplicitToken()
        {
            return await TokenInfo.getImplicitToken();
        }

    }
}
