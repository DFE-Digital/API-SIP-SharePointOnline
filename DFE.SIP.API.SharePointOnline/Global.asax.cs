using Sentry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Sentry.AspNet;

namespace DFE.SIP.API.SharePointOnline
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IDisposable _sentry;

        protected void Application_Start()
        {
            _sentry = SentrySdk.Init(o =>
            {
                // We store the DSN inside Web.config; make sure to use your own DSN!
                o.Dsn = ConfigurationManager.AppSettings["SentryDSN"];
                o.SendDefaultPii = true;
                o.AttachStacktrace = true;
                o.TracesSampleRate = 1.0;
                // Get ASP.NET integration
                o.AddAspNet();
            });

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

        protected void Application_End()
        {
            // Close the Sentry SDK (flushes queued events to Sentry)
            _sentry?.Dispose();
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
