using DFE.SIP.API.SharePointOnline.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using OfficeDevPnP.Core;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.SharePoint.Client;
using System.IO;
using File = Microsoft.SharePoint.Client.File;

namespace DFE.SIP.API.SharePointOnline.Controllers
{
    public class A2CSharePointFilesController : ApiController
    {


        private readonly string filenameSeparator = "-";


        [HttpDelete]
        // [CustomAuthorize("SpContributor")]
        // DELETE api/values/5
        public string Delete(string entityName, string recordName, string recordId, string fieldName, string fileName)
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try
            {

                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && fieldName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() &&
                      recordId.HasAValueThatIsNotAWhiteSpace() &&   fieldName.HasAValueThatIsNotAWhiteSpace()))
                    throw new Exception($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}");


                if(! (appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    throw new Exception($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings ");


                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-", "")}";

                // Authenticate against SPO with an App-Only access token
                AuthenticationManager auth = new AuthenticationManager();
                using (var context = auth.GetAppOnlyAuthenticatedContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
                appSettings.Get(appSettings.CLIENT_ID),
                appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    var rootWeb = context.Web;
                    string fileNameToSearch = fieldName + filenameSeparator + fileName;
                    context.Load(rootWeb);
                    context.ExecuteQuery();
                    File fileFound = rootWeb.GetFileByServerRelativeUrl($"{rootWeb.ServerRelativeUrl}/{sharePointLibraryName}/{sharePointFolderName}/{fileNameToSearch}");                    
                    context.Load(fileFound);
                    fileFound.DeleteObject();
                    context.ExecuteQuery();
                                        
                    return $"File {fileName} was deleted";
                }

            }
            catch (Exception ex)
            {
                //// logger.LogException(ex);
                //var valueInWebC = ConfigurationManager.AppSettings["USER_SECRETS_LOCATION"];
                //var valueInAzure = Environment.GetEnvironmentVariable("AppSettings_USER_SECRETS_LOCATION");
                //var valueInAzure2 = Environment.GetEnvironmentVariable("USER_SECRETS_LOCATION");

                //var res = $"valueInWebC |{valueInWebC}| valueInAzure |{valueInAzure}| valueInAzure2 |{valueInAzure2}|  Error Detailed {ex.Message}";
                //return new string[] { res };

                ////return $"Error CorrelationID: {logger.GetCorrelationID()}";

                logger.LogException(ex);
                return $"Error CorrelationID: {logger.GetCorrelationID()}";
            }
        }


        [HttpGet]
        // [CustomAuthorize("SpContributor")]
        // GET: api/SharePointFiles
        public string Get(string entityName, string recordName, string recordId, string fieldName)
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try
            {

                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() &&
                      recordId.HasAValueThatIsNotAWhiteSpace() &&  fieldName.HasAValueThatIsNotAWhiteSpace()))
                    throw new Exception($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}");

                if (!(appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    throw new Exception($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings ");


                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-", "")}";

                // Authenticate against SPO with an App-Only access token
                AuthenticationManager auth = new AuthenticationManager();
                using (var context = auth.GetAppOnlyAuthenticatedContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
                appSettings.Get(appSettings.CLIENT_ID),
                appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    var rootWeb = context.Web;

                    Folder folderTarget = rootWeb.GetFolderByServerRelativeUrl($"{sharePointLibraryName}/{sharePointFolderName}");
                    FileCollection files = folderTarget.Files;
                    context.Load(files);
                    context.ExecuteQuery();

                    List<string> listFileNames = new List<string>();
                    string fieldNameMarker = fieldName + filenameSeparator;
                    foreach (var file in files)
                        if (file.Name.StartsWith(fieldNameMarker))
                            listFileNames.Add(file.Name.Replace(fieldNameMarker, ""));

                    JObject result = new JObject();
                    result.Add("Files", JToken.FromObject(listFileNames));

                    return result.ToString(Newtonsoft.Json.Formatting.None);
                }

            }
            catch (Exception ex)
            {
                //// logger.LogException(ex);
                //var valueInWebC = ConfigurationManager.AppSettings["USER_SECRETS_LOCATION"];
                //var valueInAzure = Environment.GetEnvironmentVariable("AppSettings_USER_SECRETS_LOCATION");
                //var valueInAzure2 = Environment.GetEnvironmentVariable("USER_SECRETS_LOCATION");

                //var res = $"valueInWebC |{valueInWebC}| valueInAzure |{valueInAzure}| valueInAzure2 |{valueInAzure2}|  Error Detailed {ex.Message}";
                //return new string[] { res };

                ////return $"Error CorrelationID: {logger.GetCorrelationID()}";

                logger.LogException(ex);
                return $"Error CorrelationID: {logger.GetCorrelationID()}";
            }
        }


        // POST: api/SharePointFiles
        // [CustomAuthorize("SpContributor")]
        [HttpPost]
        public string Post([FromBody]string value)
        {

            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try
            {
                
                if (!value.HasAValueThatIsNotAWhiteSpace())
                    throw new Exception($"Bad Format in Request body:|{value}|");



                JObject fileUploadRequest = JObject.Parse(value);

                var entityName = fileUploadRequest["entity"].Value<string>();
                var fileName = fileUploadRequest["fileName"].Value<string>();
                var fieldName = fileUploadRequest["fieldName"].Value<string>();
                var recordName = fileUploadRequest["recordName"].Value<string>();
                var recordId = fileUploadRequest["recordId"].Value<string>();
                var fileContentBase64 = fileUploadRequest["fileContentBase64"].Value<string>();


                if (  !( entityName.HasAValueThatIsNotAWhiteSpace() &&   recordName.HasAValueThatIsNotAWhiteSpace() &&
                         recordId.HasAValueThatIsNotAWhiteSpace() &&     fileName.HasAValueThatIsNotAWhiteSpace() &&
                         (!String.IsNullOrEmpty(fileContentBase64)) &&   fieldName.HasAValueThatIsNotAWhiteSpace()) )
                    throw new Exception($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}");


                if (!(appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    throw new Exception($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings ");




                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-","")}";
                var sharePointFileName = $"{fieldName}{filenameSeparator}{fileName}";
                               


                AuthenticationManager auth = new AuthenticationManager();

                using (var context = auth.GetAppOnlyAuthenticatedContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
               appSettings.Get(appSettings.CLIENT_ID),
               appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    var rootWeb = context.Web;
                    //context.Load(rootWeb, web => web.Title);

                    Folder folderTarget = rootWeb.GetFolderByServerRelativeUrl($"{sharePointLibraryName}/{sharePointFolderName}");
                    context.Load(folderTarget);
                    context.ExecuteQuery();

                    FileCreationInformation newFile = new FileCreationInformation();
                    var bytes = Convert.FromBase64String(fileContentBase64);

                    newFile.ContentStream = new MemoryStream(bytes);
                    newFile.Url = sharePointFileName;
                    newFile.Overwrite = true;

                    var uploadRepresentationOfFile = folderTarget.Files.Add(newFile);
                    var itemRepresentationOFile = uploadRepresentationOfFile.ListItemAllFields;
                    itemRepresentationOFile.Update();
                    context.Load(itemRepresentationOFile);
                    context.ExecuteQueryRetry(2);

                    return fileName;
                }
               
            }
            catch (Exception ex)
            {
                 logger.LogException(ex);
                 return $"Error CorrelationID: {logger.GetCorrelationID()}";
            }


        }

      



    }
}
