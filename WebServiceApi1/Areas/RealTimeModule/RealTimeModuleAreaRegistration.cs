using System.Web.Mvc;

namespace WebServiceApi1.Areas.RealTimeModule
{
    public class RealTimeModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RealTimeModule";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RealTimeModule_default",
                "RealTimeModule/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}