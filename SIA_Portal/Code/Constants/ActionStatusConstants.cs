using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Constants
{
    public class ActionStatusConstants
    {

        public const int STATUS_NO_ACTION = 0;
        public const int STATUS_SUCCESS = 1;
        public const int STATUS_FAILED = 2;


        public static string GetColorAsStringAssociatedWithStatus(int status)
        {
            if (status == STATUS_SUCCESS)
            {
                return "color:green";
            }
            else if (status == STATUS_FAILED)
            {
                return "color:darkred";
            }
            else
            {
                return "color:black";
            }
        }

    }
}