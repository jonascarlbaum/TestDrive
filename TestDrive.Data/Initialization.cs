using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace TestDrive.Data
{
    #if DEBUG
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class Initialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(name: "Database Route",
                url: "database/{action}",
                defaults: new { action = "index", controller = "Database" });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
    #endif
}
