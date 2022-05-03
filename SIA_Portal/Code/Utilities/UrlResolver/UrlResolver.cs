using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Utilities.UrlResolver
{
    public static class UrlResolver
    {

        public static string RelativePath(string path, HttpRequest context)
        {
            return path.Replace(context.ServerVariables["APPL_PHYSICAL_PATH"], "~/").Replace(@"\", "/");
        }

    }
}