using System.Data.Entity.Migrations;
using System.Web.Http;
using System.Web.Routing;

namespace LeagueSeason5CountersService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();

            var configuration = new Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}