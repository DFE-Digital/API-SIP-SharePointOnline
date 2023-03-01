using DFE.SIP.API.SharePointOnline.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Microsoft.SharePoint.Client;
using System.IO;
using File = Microsoft.SharePoint.Client.File;
using System.Threading.Tasks;
using System.Net;
using HttpDeleteAttribute = System.Web.Http.HttpDeleteAttribute;
using AuthenticationManager = PnP.Framework.AuthenticationManager;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DFE.SIP.API.SharePointOnline.Controllers
{
    public class A2CSharePointFilesController : ApiController
    {


        private readonly string filenameSeparator = "-";


        [HttpDelete]
        [CustomAuthorize("SpContributor")]
        // DELETE api/values/5
        public async Task<HttpResponseMessage> Delete(string entityName, string recordName, string recordId, string fieldName, string fileName)
        { AppSettingsManager appSettings = new AppSettingsManager();
           LogOperations logger = new LogOperations(appSettings);

            try
            {

                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() && recordId.HasAValueThatIsNotAWhiteSpace() &&
                      fieldName.HasAValueThatIsNotAWhiteSpace() && fileName.HasAValueThatIsNotAWhiteSpace()))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };

   


                if(! (appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings "),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                


                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-", "")}";

                // Authenticate against SPO with an App-Only access token
                AuthenticationManager auth = new AuthenticationManager();
                using (var context = auth.GetACSAppOnlyContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
                appSettings.Get(appSettings.CLIENT_ID),
                appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    var rootWeb = context.Web;
                    string fileNameToSearch = fieldName + filenameSeparator + fileName;
                    context.Load(rootWeb);
                    await context.ExecuteQueryRetryAsync(2);
                    File fileFound = rootWeb.GetFileByServerRelativeUrl($"{rootWeb.ServerRelativeUrl}/{sharePointLibraryName}/{sharePointFolderName}/{fileNameToSearch}");                    
                    context.Load(fileFound);
                    fileFound.DeleteObject();
                    await context.ExecuteQueryRetryAsync(2);

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"File {fileName} was deleted"),
                        StatusCode = HttpStatusCode.OK
                    };

                }

            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"File {fileName} not found."),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                else
                {
                    logger.LogException(ex);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    
                    
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new HttpResponseMessage()
                {
                    Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                    StatusCode = HttpStatusCode.BadRequest
                };
                
            }
        }


        [HttpGet]
        [CustomAuthorize("SpContributor")]
        // GET: api/SharePointFiles
        public async Task<HttpResponseMessage> Get(string entityName, string recordName, string recordId, string fieldName)
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try
            {
                logger.LogEvent($"Getting a file {entityName}|{recordName}|{recordId}|{fieldName}\"");

                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() &&
                      recordId.HasAValueThatIsNotAWhiteSpace() &&  fieldName.HasAValueThatIsNotAWhiteSpace()))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };

                

                if (!(appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings "),
                        StatusCode = HttpStatusCode.BadRequest
                    };

                
                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-", "")}";

                // Authenticate against SPO with an App-Only access token
                AuthenticationManager auth = new AuthenticationManager();
                using (var context = auth.GetACSAppOnlyContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
                appSettings.Get(appSettings.CLIENT_ID),
                appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    var rootWeb = context.Web;

                    Folder folderTarget = rootWeb.GetFolderByServerRelativeUrl($"{sharePointLibraryName}/{sharePointFolderName}");
                    FileCollection files = folderTarget.Files;
                    context.Load(files);
                    await context.ExecuteQueryRetryAsync(2);

                    List<string> listFileNames = new List<string>();
                    string fieldNameMarker = fieldName + filenameSeparator;
                    foreach (var file in files)
                        if (file.Name.StartsWith(fieldNameMarker))
                            listFileNames.Add(file.Name.Replace(fieldNameMarker, ""));

                    JObject result = new JObject();
                    result.Add("Files", JToken.FromObject(listFileNames));


                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"{result.ToString(Newtonsoft.Json.Formatting.None)}"),
                        StatusCode = HttpStatusCode.OK
                    };

                }

            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {                    
                    // no files found.
                    JObject result = new JObject();
                    result.Add("Files", JToken.FromObject(new List<String>()));

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"{result.ToString(Newtonsoft.Json.Formatting.None)}"),
                        StatusCode = HttpStatusCode.OK
                    };

                    
                }
                    
                else
                {
                    logger.LogException(ex);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new HttpResponseMessage()
                {
                    Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                    StatusCode = HttpStatusCode.BadRequest
                };                

            }
        }



        [HttpGet]
        [CustomAuthorize("SpContributor")]
        // GET: api/SharePointFiles
        public async Task<HttpResponseMessage> Download(string entityName, string recordName, string recordId, string relativePath = null, string fieldName = null, string fileName = null)
        {
            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);
            string calculatedUrl = "";

            logger.LogEvent($"Download1 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

            try
            {
                logger.LogEvent($"Download2 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() &&
                      recordId.HasAValueThatIsNotAWhiteSpace() ))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };


                logger.LogEvent($"Download3 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                if (!(appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings "),
                        StatusCode = HttpStatusCode.BadRequest
                    };

                logger.LogEvent($"Download4 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                var sharePointLibraryName = $"{entityName}";
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-", "")}";

                logger.LogEvent($"Download5 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                // Authenticate against SPO with an App-Only access token
                AuthenticationManager auth = new AuthenticationManager();
                using (var context = auth.GetACSAppOnlyContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
                appSettings.Get(appSettings.CLIENT_ID),
                appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                    logger.LogEvent($"Download6 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                    var rootWeb = context.Web;


                    if (!String.IsNullOrEmpty(relativePath) )
                    {

                        logger.LogEvent($"Download7 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                        context.Load(rootWeb);

                        logger.LogEvent($"Download8 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                        await context.ExecuteQueryRetryAsync(2);

                        logger.LogEvent($"Download9 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                        calculatedUrl = (rootWeb.ServerRelativeUrl.EndsWith("/") ? rootWeb.ServerRelativeUrl : rootWeb.ServerRelativeUrl + "/") +
                                                $"{sharePointLibraryName}/{sharePointFolderName}{relativePath}";

                        logger.LogEvent($"Download10 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}|{calculatedUrl}");

                        File file = rootWeb.GetFileByServerRelativeUrl(calculatedUrl);
                        var stream = file.OpenBinaryStream();
                        context.Load(file);
                        await context.ExecuteQueryRetryAsync(2);

                        logger.LogEvent($"Download11 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}|{calculatedUrl}");


                        using (var reader = new StreamReader(stream.Value, Encoding.UTF8))
                        {
                            logger.LogEvent($"Download12 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}|{calculatedUrl}");
                            string result = reader.ReadToEnd();
                            

                            logger.LogEvent($"Download13 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}|{calculatedUrl}|{result.Length}");

                            return new HttpResponseMessage()
                            {
                                Content = new StringContent(result),
                                StatusCode = HttpStatusCode.OK
                            };

                           
                        }
                    }

                    logger.LogEvent($"Download14 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");
                    //Not Found
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($""),
                        StatusCode = HttpStatusCode.NotFound
                    };

                }

            }
            catch (ServerException ex)
            {

                //logger.LogInformation($"Download15 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");
                //logger.LogInformation($"Download16 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                //logger.LogInformation($"Download17 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}",
                //    new[] { ("Exception",$"{ex.Message}")});

                Exception extendedException = new Exception($"Calculated URL:{calculatedUrl}:original Exception msg:{ex.Message}");
                 logger.LogException(extendedException);
                

                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    // no files found.
                    JObject result = new JObject();
                    result.Add("Files", JToken.FromObject(new List<string>()));

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"{result.ToString(Newtonsoft.Json.Formatting.None)}"),
                        StatusCode = HttpStatusCode.OK
                    };


                }

                else
                {
                  //  logger.LogException(ex);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };

                }
            }
            catch (Exception ex)
            {

                //logger.LogInformation($"Download18 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");
                //logger.LogInformation($"Download19 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}");

                //logger.LogInformation($"Download120 {entityName}|{recordName}|{recordId}|{relativePath}|{fileName}",
                //    new[] { ("Exception", $"{ex.Message}") });

                Exception extendedException = new Exception($"Calculated URL:{calculatedUrl}:original Exception msg:{ex.Message}");
                logger.LogException(extendedException);

                return new HttpResponseMessage()
                {
                    Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                    StatusCode = HttpStatusCode.BadRequest
                };

            }
        }





        // POST: api/SharePointFiles
        [CustomAuthorize("SpContributor")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {

            

            AppSettingsManager appSettings = new AppSettingsManager();
            LogOperations logger = new LogOperations(appSettings);

            try
            {
                
                if (!value.HasAValueThatIsNotAWhiteSpace())
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Bad Format in Request body:|{value}|"),
                        StatusCode = HttpStatusCode.BadRequest
                    };

                JObject fileUploadRequest = JObject.Parse(value);

                var entityName = fileUploadRequest["entity"].Value<string>();
                var fileName = fileUploadRequest["fileName"].Value<string>();
                var fieldName = fileUploadRequest["fieldName"].Value<string>();
                var recordName = fileUploadRequest["recordName"].Value<string>();
                var recordId = fileUploadRequest["recordId"].Value<string>();
                var fileContentBase64 = fileUploadRequest["fileContentBase64"].Value<string>();


                if (!(entityName.HasAValueThatIsNotAWhiteSpace() && recordName.HasAValueThatIsNotAWhiteSpace() &&
                         recordId.HasAValueThatIsNotAWhiteSpace() && fileName.HasAValueThatIsNotAWhiteSpace() &&
                         (!String.IsNullOrEmpty(fileContentBase64)) && fieldName.HasAValueThatIsNotAWhiteSpace()))
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"Bad Format in Request parameters expected values for entity,recordName,recordId,fieldName :{entityName},{recordName},{recordId},{fieldName}"),
                        StatusCode = HttpStatusCode.BadRequest
                    };


                if (!(appSettings.Get(appSettings.A2CEntitiesAllowedToCRUDFiles).Split(',')).Contains(entityName))
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent($"EntityName {entityName} not allowed because it is now present in A2CEntitiesAllowedToCRUDFiles appsettings "),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
             
                var sharePointLibraryNameInURL = $"{entityName}"; // ListName is actually diferent and SharePointOnlineUtilities.A2CConvertDynamicsEntityNameToListName is used to get its value.
                var sharePointFolderName = $"{recordName.ToUpper()}_{recordId.ToUpper().Replace("-","")}";
                var sharePointFileName = $"{fieldName}{filenameSeparator}{fileName}";
                               


                AuthenticationManager auth = new AuthenticationManager();

                using (var context = auth.GetACSAppOnlyContext(appSettings.Get(appSettings.SharePointSiteCollectionUrl),
               appSettings.Get(appSettings.CLIENT_ID),
               appSettings.Get(appSettings.CLIENT_SECRET)))
                {

                   // var rootWeb = context.Web;
                    
                    Folder folderTarget = await SharePointOnlineUtilities.ensureFolderExistsAsync($"{sharePointLibraryNameInURL}/{sharePointFolderName}",sharePointFolderName,
                       SharePointOnlineUtilities.A2CConvertDynamicsEntityNameToListName(sharePointLibraryNameInURL),
                       context);

                    FileCreationInformation newFile = new FileCreationInformation();
                    var bytes = Convert.FromBase64String(fileContentBase64);

                    newFile.ContentStream = new MemoryStream(bytes);
                    newFile.Url = sharePointFileName;
                    newFile.Overwrite = true;

                    var uploadRepresentationOfFile = folderTarget.Files.Add(newFile);
                    var itemRepresentationOFile = uploadRepresentationOfFile.ListItemAllFields;
                    itemRepresentationOFile.Update();
                    context.Load(itemRepresentationOFile);
                    await context.ExecuteQueryRetryAsync(2);


                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(fileName),
                        StatusCode = HttpStatusCode.OK
                    };
                    
                  
                }
               
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return new HttpResponseMessage()
                {
                    Content = new StringContent($"Error CorrelationID: {logger.GetCorrelationId()}"),
                    StatusCode = HttpStatusCode.BadRequest
                };             
                               
            }


        }

      



    }
}
