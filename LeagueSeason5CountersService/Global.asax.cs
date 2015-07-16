using System.Web.Http;
using System.Web.Routing;

namespace LeagueSeason5CountersService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}