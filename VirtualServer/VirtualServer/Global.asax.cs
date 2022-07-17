using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace VirtualServer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //var t = AppDomain.CurrentDomain.GetData();
            //if (Directory.Exists(AppDomain.CurrentDomain.GetData())
            //{

            //}
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
