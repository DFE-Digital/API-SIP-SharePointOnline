using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DFE.SIP.API.SharePointOnline.Utilities
{
    public static class StringExtensions
    {

        public static bool HasAValueThatIsNotAWhiteSpace(this string str)
        {

            return (!(string.IsNullOrEmpty(str) || (string.IsNullOrWhiteSpace(str))));
        }

        public static bool HasANullStringValue(this string str)
        {
            if (str.HasAValueThatIsNotAWhiteSpace())
                return (String.Compare(str, "null", true) == 0);

            return false;
        }


        public static bool HasAGuidValue(this string str)
        {
            Guid valueG;

            if (str.HasAValueThatIsNotAWhiteSpace())
                return Guid.TryParse(str, out valueG);

            return false;
        }


        public static string ConvertToEmptyStringIfNull(this string str)
        {

            if (str == null)
                return "";

            return str;
        }

    }
}