using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Constants
{
    public class TypeConstants
    {
        /*
        //UNUSED
        public const string ACCOUNT_TYPE_EMPLOYEE = "SIA_EMPLOYEE_ACC_TYPE";
        //UNUSED
        public const string ACCOUNT_TYPE_APPLICANT = "SIA_APPLICANT_ACC_TYPE";
        //UNUSED
        public const string ACCOUNT_TYPE_ADMIN_HIGHEST_LEVEL = "SIA_HIGHEST_LEVEL_ADMIN_ACC_TYPE";
        */

        public const string ACCOUNT_TYPE_NORMAL = "Normal";



        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountType"></param>
        /// <returns>The name of the given type. If the type does not correspond to any existing type,
        /// then an empty string is returned</returns>
        public static string GetAccountNameFromType(string accountType)
        {
            if (accountType != null)
            {
                /*
                if (accountType.Equals(ACCOUNT_TYPE_ADMIN_HIGHEST_LEVEL))
                {
                    return "Admin";
                }
                else if (accountType.Equals(ACCOUNT_TYPE_EMPLOYEE))
                {
                    return "Employee";
                }
                else if (accountType.Equals(ACCOUNT_TYPE_APPLICANT))
                {
                    return "Applicant";
                }
                else
                {
                    return "";
                }
                */
                return "";
            }
            else
            {
                return "";
            }
        }

    }
}