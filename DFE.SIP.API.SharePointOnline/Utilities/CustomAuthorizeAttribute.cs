using Microsoft.SharePoint.Client.RecordsRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace DFE.SIP.API.SharePointOnline.Utilities
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        // Entities context = new Entities(); // my entity  
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }


        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try {
                logger.LogEvent($"Authorizing user");

                var identity = (ClaimsIdentity)actionContext.RequestContext.Principal.Identity;
                var id = identity.Claims.FirstOrDefault(c => c.Type == "appid")?.Value;

                var spAuthorizedAppIds = ConfigurationManager.AppSettings["SPAuthorizedAppsIDs"];

                logger.LogEvent($"Authorizing user, appid: {id}");
                logger.LogEvent($"Authorizing against configuration SPAuthorizedAppsIDs: {spAuthorizedAppIds}");

                return id != null && ConfigurationManager.AppSettings["SPAuthorizedAppsIDs"].Contains(id); //              
            }
            catch (Exception) {
                return false;
            }



            
        }


        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    var UserID = httpContext.User.Identity.Name;

        //    return false;

        //}
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    filterContext.Result = new HttpUnauthorizedResult();




        //}
    }
}