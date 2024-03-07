using DFE.SIP.API.SharePointOnline.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DFE.SIP.API.SharePointOnline.Controllers
{

    public static class A2CHealthEvents
    {

        public static string CheckHealth = "CheckHealth";
        public static string CheckHealthInvalidKey = "CheckHealthInvalidKey";

    }

    public class HealthInternalStatusOnlyController : ApiController
    {
               

        [HttpGet]
        public string Get()
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            return "BuildVersion:" + appSettings.Get(appSettings.BuildVersion);
        }
    }
}
