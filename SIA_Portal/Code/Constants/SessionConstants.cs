using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Constants
{
    public class SessionConstants
    {

        public const string SESSION_ACCOUNT_OBJ = "Portal_LoggedInAccount";

        public const string SESSION_SELECTED_ADMIN_ACCOUNTS = "Portal_SelectedAdminAccounts";
        public const string SESSION_SELECTED_EMPLOYEE_ACCOUNTS = "Portal_SelectedEmployeeAccounts";
        public const string SESSION_SELECTED_ANNOUNCEMENTS = "Portal_SelectedAnnouncements";

        public static void RemoveSessionObjectsFromSession(System.Web.HttpContext context)
        {
            context.Session.Remove(SESSION_ACCOUNT_OBJ);

            context.Session.Remove(SESSION_SELECTED_ADMIN_ACCOUNTS);
            context.Session.Remove(SESSION_SELECTED_EMPLOYEE_ACCOUNTS);
            context.Session.Remove(SESSION_SELECTED_ANNOUNCEMENTS);
        }

    }
}