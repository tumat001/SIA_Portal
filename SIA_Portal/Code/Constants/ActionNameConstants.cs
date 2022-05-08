using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Constants
{
    public class ActionNameConstants
    {

        public const string REGIS_NOT_REQUIRED__LOG_IN_PAGE = "RegisNotRequired_LogInPage";
        public const string REGIS_NOT_REQUIRED__LOG_IN_PAGE__EXECUTE_ACTION = "RegisNotRequired_LogInPage_ExecuteAction";

        //

        public const string REGIS_NOT_REQUIRED__REGISTER_PAGE = "RegisNotRequired_RegisterPage";
        public const string REGIS_NOT_REQUIRED__REGISTER_PAGE__EXECUTE_ACTION = "RegisNotRequired_RegisterPage_ExecuteAction";

        //

        public const string EMPLOYEE_SIDE__HOME_PAGE = "Employee_HomePage";
        public const string EMPLOYEE_SIDE__HOME_PAGE__EXECUTE_ACTION = "Employee_HomePage_ExecuteAction";

        //

        public const string ADMIN_SIDE__HOME_PAGE = "Admin_HomePage";
        public const string ADMIN_SIDE__HOME_PAGE__EXECUTE_ACTION = "Admin_HomePage_ExecuteAction";

        public const string ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO = "Admin_ManageAdminAccountsPage";
        public const string ADMIN_SIDE__MANAGE_ADMIN_PAGE__EXECUTE_ACTION = "Admin_ManageAdminAccountsPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO = "Admin_CreateEditAdminAccountPage";
        public const string ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__EXECUTE_ACTION = "Admin_CreateEditAdminAccountPage_ExecuteAction";

        public const string ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__GO_TO = "Admin_ConfirmDeleteSelectedAdminPage";
        public const string ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__EXECUTE_ACTION = "Admin_ConfirmDeleteSelectedAdminPage_ExecuteAction";


        //

        public const string ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO = "Admin_ManageEmployeeAccountsPage";
        public const string ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__EXECUTE_ACTION = "Admin_ManageEmployeeAccountsPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO = "Admin_CreateEditEmployeeAccountPage";
        public const string ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__EXECUTE_ACTION = "Admin_CreateEditEmployeeAccountPage_ExecuteAction";

        public const string ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__GO_TO = "Admin_ConfirmDeleteSelectedEmployeePage";
        public const string ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__EXECUTE_ACTION = "Admin_ConfirmDeleteSelectedEmployeePage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO = "Admin_ManageAnnouncementsPage";
        public const string ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__EXECUTE_ACTION = "Admin_ManageAnnouncementsPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__GO_TO = "Admin_CreateEditAnnouncementPage";
        public const string ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__EXECUTE_ACTION = "Admin_CreateEditAnnouncementPage_ExecuteAction";

        public const string ADMIN_SIDE__ANNOUNCEMENT_CATEGORY__QUERY_ACTION = "Admin_AnnouncementCategory_QueryAction";

        public const string ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__GO_TO = "Admin_ConfirmDeleteSelectedAnnouncementPage";
        public const string ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__EXECUTE_ACTION = "Admin_ConfirmDeleteSelectedAnnouncementPage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_PERMISSION_PAGE__GO_TO = "Admin_ManagePermissionsPage";
        public const string ADMIN_SIDE__MANAGE_PERMISSION_PAGE__EXECUTE_ACTION = "Admin_ManagePermissionsPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__GO_TO = "Admin_CreateEditPermissionPage";
        public const string ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__EXECUTE_ACTION = "Admin_CreateEditPermissionPage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO = "Admin_ManageRequestableDocumentsPage";
        public const string ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION = "Admin_ManageRequestableDocumentsPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO = "Admin_CreateEditRequestableDocumentsPage";
        public const string ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION = "Admin_CreateEditRequestableDocumentsPage_ExecuteAction";

        public const string ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__GO_TO = "Admin_ConfirmDeleteSelectedRequestableDocumentPage";
        public const string ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION = "Admin_ConfirmDeleteSelectedRequestableDocumentPage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO = "Admin_ManageAccountDivisionPage";
        public const string ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION = "Admin_ManageAccountDivisionPage_ExecuteAction";

        public const string ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__GO_TO = "Admin_CreateEditAccountDivisionPage";
        public const string ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION = "Admin_CreateEditAccountDivisionPage_ExecuteAction";

        public const string ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__GO_TO = "Admin_ConfirmDeleteSelectedAccountDivisionPage";
        public const string ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION = "Admin_ConfirmDeleteSelectedAccountDivisionPage_ExecuteAction";

        //

        public const string ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__GO_TO = "Admin_ConfigurePermissionsOfAccountPage";
        public const string ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__EXECUTE_ACTION = "Admin_ConfigurePermissionsOfAccountPage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_ManageReqDocuQueuePage";
        public const string ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_ManageReqDocuQueuePage_ExecuteAction";

        public const string ADMIN_SIDE__MANAGE_REQ_DOCU_QUEUE_PAGE__CHANGE_QUEUE_STATUS_FILTER = "Admin_ManageReqDocuQueuePage_ChangeStatusFilter";

        public const string ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_TakeActionReqDocuQueuePage";
        public const string ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_TakeActionReqDocuQueuePage_ExecuteAction";

        public const string ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_TerminateReqDocuQueuePage";
        public const string ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_TerminateReqDocuQueuePage_ExecuteAction";

        //

        public const string ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_ManageAcknowledgedReqDocuQueuePage";
        public const string ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_ManageAcknowledgedReqDocuQueuePage_ExecuteAction";

        public const string ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_FulfillAcknowledgedReqDocuQueuePage";
        public const string ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_FulfillAcknowledgedReqDocuQueuePage_ExecuteAction";

        public const string ADMIN_SIDE__TERMINATE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO = "Admin_TerminateAcknowledgedReqDocuQueuePage";
        public const string ADMIN_SIDE__TERMINATE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION = "Admin_TerminateAcknowledgedReqDocuQueuePage_ExecuteAction";


        //

        public const string GENERIC_SIDE_NAVBAR__EXECUTE_ACTION = "GenericSide_NavBar_ExecuteAction";

        public const string GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO = "GenericUser_ViewEditEmployeeRecordsPage";
        public const string GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__EXECUTE_ACTION = "GenericUser_ViewEditEmployeeRecordsPage_ExecuteAction";

        public const string GENERIC_USER__ANNOUNCEMENT_PAGE__GO_TO = "GenericUser_ViewAnnouncementPage";

        public const string GENERIC_USER__CHANGE_OWN_PASSWORD__GO_TO = "GenericUser_ChangeOwnPasswordPage";
        //public const string GENERIC_USER__CHANGE_OWN_PASSWORD__EXECUTE_ACTION = "GenericUser_ChangeOwnPasswordPage_ExecuteAction";

        public const string GENERIC_USER__MUST_CHANGE_CREDENTIALS__GO_TO = "GenericUser_MustChangeOwnUsernamePasswordPage";
        //public const string GENERIC_USER__MUST_CHANGE_CREDENTIALS__EXECUTE = "GenericUser_MustChangeOwnUsernamePasswordPage_ExecuteAction";


        public const string GENERIC_USER__REQUEST_FOR_DOCUMENT__GO_TO = "GenericUser_RequestDocumentPage";
        public const string GENERIC_USER__REQUEST_FOR_DOCUMENT__EXECUTE_ACTION = "GenericUser_RequestDocumentPage_ExecuteAction";

        public const string GENERIC_USER__REQUEST_DOCUMENT_ID_SELECTED = "GenericUser_ReqDocuIdSelectedFromSelectList";


        public const string GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO = "GenericUser_ManageNotificationPage_GoTo";
        public const string GENERIC_USER__MANAGE_NOTIFICATION_PAGE__EXECUTE_ACTION = "GenericUser_ManageNotificationPage_ExecuteAction";

        public const string GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__GO_TO = "GenericUser_ViewSingleNotificationPage";
        public const string GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__EXECUTE_ACTION = "GenericUser_ViewSingleNotificationPage_ExecuteAction";


    }
}