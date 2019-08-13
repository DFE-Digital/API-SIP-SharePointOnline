using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public class AppSettingsManager 
    {



        public readonly string USER_SECRETS_LOCATION = "USER_SECRETS_LOCATION";
        public readonly string CLIENT_ID = "CLIENT_ID";
        public readonly string CLIENT_SECRET = "CLIENT_SECRET";
        public readonly string SharePointSiteCollectionUrl = "SharePointSiteCollectionUrl";
        public readonly string APPINSIGHTS_KEY = "APPINSIGHTS_KEY";
        public readonly string Environment = "Environment";
        public readonly string A2CEntitiesAllowedToCRUDFiles = "A2CEntitiesAllowedToCRUDFiles";
        public readonly string BuildVersion = "BuildVersion";
        

        public AppSettingsManager() { }
        

        public  string Get(string settingName)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[USER_SECRETS_LOCATION])) // for local Development
            {

                try
                {
                    // try load json file if doesnt exist then reads from COnfigurationManager
                    JObject o1 = JObject.Parse(File.ReadAllText(ConfigurationManager.AppSettings[USER_SECRETS_LOCATION].ToString()));
                    if (o1[settingName] == null)
                    {
                        if (ConfigurationManager.AppSettings[settingName] == null)
                        {
                            throw new ArgumentException("Property does not exist, does not have a value, or a test setting is not selected. Check User Secrets File.");
                        }
                        else
                        {
                            return ConfigurationManager.AppSettings[settingName];
                        }
                    }
                    return o1[settingName].ToString();
                }
                catch (Exception ex)
                {
                    // if it gets here then Configuration is really wrong - if local development a USER_SECRETS_LOCATION json file should exist, if in an Azure environement ConfigurationManager should be able to find the setting from an Environment variable
                    throw ex;
                }
            }
            else // An Azure environment should always execute this part
            {
                if (ConfigurationManager.AppSettings[settingName] == null)
                {
                    throw new ArgumentException("Property does not exist, does not have a value, or a test setting is not selected");
                }
                else
                {
                    return ConfigurationManager.AppSettings[settingName];
                }
            }


        }


    }
}