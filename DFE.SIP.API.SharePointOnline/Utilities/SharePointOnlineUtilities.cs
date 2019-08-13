using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public static class SharePointOnlineUtilities
    {



        public static string A2CConvertDynamicsEntityNameToListName(string entityName)
        {

            switch (entityName)
            { 
                 case "sip_application":
                    return "Application";                    

                 case "sip_applyingschools":
                    return "Applying School";
                    

                default:
                    throw new Exception($"EntityName {entityName} has not been mapped to a SharePoint ListName.");

            }


            
        }




        public static async Task<Folder>  ensureFolderExistsAsync(string folderPathSiteRelative, string folderName, string libraryName,  ClientContext context)
        {

            Folder folderTarget = null;
            var rootWeb = context.Web;
            context.Load(rootWeb);
            await Task.Run(() => context.ExecuteQueryRetryAsync(2));

            try
            {
                folderTarget = rootWeb.GetFolderByServerRelativeUrl(folderPathSiteRelative);
                context.Load(folderTarget);               
                await Task.Run(() => context.ExecuteQueryRetryAsync(2));
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    folderTarget = null;
                }

            }


            if (folderTarget == null)
            {
                // create folder

                List list = rootWeb.Lists.GetByTitle(libraryName);
                var folder = list.RootFolder;
                rootWeb.Context.Load(list);
                rootWeb.Context.Load(folder);
                await Task.Run(() => rootWeb.Context.ExecuteQueryRetryAsync(2));

              
                
                    ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                    newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                    newItemInfo.LeafName = folderName;
                    ListItem newListItem = list.AddItem(newItemInfo);
                    newListItem["Title"] = folderName;                    
                    newListItem.Update();

                await Task.Run(() => rootWeb.Context.ExecuteQueryRetryAsync(2));


                folderTarget = rootWeb.GetFolderByServerRelativeUrl(folderPathSiteRelative);
                context.Load(folderTarget);
                await Task.Run(() => rootWeb.Context.ExecuteQueryRetryAsync(2));

            }

            return folderTarget;                                                  

        }


    }
}