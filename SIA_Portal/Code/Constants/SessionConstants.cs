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
        public const string SESSION_SELECTED_PERMISSIONS = "Portal_SelectedPermissions";
        public const string SESSION_SELECTED_REQUESTABLE_DOCUMENTS = "Portal_SelectedRequestableDocuments";
        public const string SESSION_SELECTED_ACCOUNT_DIVISION = "Portal_SelectedAccountDivision";
        public const string SESSION_SELECTED_REQ_DOCU_QUEUE = "Portal_SelectedReqDocuQueue";
        public const string SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE = "Portal_SelectedAcknowledgedReqDocuQueue";
        public const string SESSION_SELECTED_NOTIFICATIONS = "Portal_SelectedNotification";

        public static void RemoveSessionObjectsFromSession(System.Web.HttpContext context)
        {
            context.Session.Remove(SESSION_ACCOUNT_OBJ);

            context.Session.Remove(SESSION_SELECTED_ADMIN_ACCOUNTS);
            context.Session.Remove(SESSION_SELECTED_EMPLOYEE_ACCOUNTS);
            context.Session.Remove(SESSION_SELECTED_ANNOUNCEMENTS);
            context.Session.Remove(SESSION_SELECTED_PERMISSIONS);
            context.Session.Remove(SESSION_SELECTED_REQUESTABLE_DOCUMENTS);
            context.Session.Remove(SESSION_SELECTED_ACCOUNT_DIVISION);
            context.Session.Remove(SESSION_SELECTED_REQ_DOCU_QUEUE);
            context.Session.Remove(SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE);
            context.Session.Remove(SESSION_SELECTED_NOTIFICATIONS);

        }

    }
}