using System;
using System.Reflection;

namespace DFE.SIP.API.SharePointOnline.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}