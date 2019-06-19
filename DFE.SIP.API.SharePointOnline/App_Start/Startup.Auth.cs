using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System.Configuration;
//using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace DFE.SIP.API.SharePointOnline
{
    public partial class Startup
    {

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
                    }
                   // AuthenticationType = "OAuth2Bearer"
                });
        }

    }
}