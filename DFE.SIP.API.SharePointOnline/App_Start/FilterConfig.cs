using System.Web;
using System.Web.Mvc;

namespace DFE.SIP.API.SharePointOnline
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
