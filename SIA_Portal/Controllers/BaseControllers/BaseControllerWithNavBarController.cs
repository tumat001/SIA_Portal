using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIA_Portal.Constants;
using SIA_Portal.Models.RegisNotRequired;
using SIA_Portal.Models.BaseModels;
using CommonDatabaseActionReusables.AccountManager;

namespace SIA_Portal.Controllers.BaseControllers
{
    public class BaseControllerWithNavBarController : Controller
    {

        #region "Nav bar"

        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_SIDE_NAVBAR__EXECUTE_ACTION)]
        public ActionResult SideNavBar_ExecuteAction(BaseAccountLoggedInModel model, string executeAction)
        {
            if (executeAction != null)
            {
                if (executeAction.Equals("LogOut"))
                {
                    return LogOut();
                }
                else if (executeAction.Equals("GoToManageAdminAccount"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("CreateAdminAccount"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("GoToManageEmployeeAccount"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("CreateEmployeeAccount"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("GoToManageAnnouncement"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("GoToEditSelfEmployeeRecord"))
                {
                    return GoTo_EditSelfEmployeeRecords();
                }
                else if (executeAction.Equals("GoToAdminHomePage"))
                {
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("GoToEmployeeHomePage"))
                {
                    return RedirectToAction(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE, ControllerNameConstants.EMPLOYEE_CONTROLLER_NAME);
                }
                else if (executeAction.Equals("GoToChangeOwnPassword"))
                {
                    return RedirectToAction(ActionNameConstants.GENERIC_USER__CHANGE_OWN_PASSWORD__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
                }
            }

            return new ContentResult(); //Error, should not reach here.
        }

        private ActionResult LogOut()
        {
            //If something is changed here, change code in GenericUserController, in credential change.
            SessionConstants.RemoveSessionObjectsFromSession(System.Web.HttpContext.Current);

            return RedirectToAction(ActionNameConstants.REGIS_NOT_REQUIRED__LOG_IN_PAGE, ControllerNameConstants.REGIS_NOT_REQUIRED_CONTROLLER_NAME);
        }

        //Edit the one in employee controller as well
        protected ActionResult GoTo_EditSelfEmployeeRecords()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var selfAccId = account.Id;
            TempData.Add(GenericUserController.TEMP_DATA_EMPLOYEE_ID_KEY, selfAccId);

            return RedirectToAction(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }


        #endregion

    }
}