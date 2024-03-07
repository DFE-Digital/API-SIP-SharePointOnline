using Sentry;
using Sentry.AspNet;
using System.Web.Http;
using System.Web.Mvc;

namespace DFE.SIP.API.SharePointOnline
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
           // RouteConfig.RegisterRoutes(RouteTable.Routes);
           // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Global error catcher
        protected void Application_Error()
        {
            var exception = Server.GetLastError();

            // Capture unhandled exceptions
            SentrySdk.CaptureException(exception);
        }

        protected void Application_BeginRequest()
        {
            // Start a transaction that encompasses the current request
            Context.StartSentryTransaction();
        }

        protected void Application_EndRequest()
        {
            Context.FinishSentryTransaction();
        }
    }
}
