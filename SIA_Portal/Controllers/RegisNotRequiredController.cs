using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIA_Portal.Constants;
using SIA_Portal.Models.RegisNotRequired;
using SIA_Portal.Accessors;
using SIA_Portal.Controllers;
using CommonDatabaseActionReusables.AccountManager;
using CommonDatabaseActionReusables.AccountManager.Actions.Exceptions;

namespace SIA_Portal.Controllers
{
    public class RegisNotRequiredController : Controller
    {

        private const string TEMP_MODEL_KEY = "TempModelKey";

        private PortalAccountAccessor accountAccessor = new PortalAccountAccessor();
        private PortalAccountMustChangeCredentialsAccessor accMustChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();


        #region "Log In"

        [ActionName(ActionNameConstants.REGIS_NOT_REQUIRED__LOG_IN_PAGE)]
        public ActionResult Index()
        {
            var model = (RegisNotRequired_LogInModel) TempData[TEMP_MODEL_KEY];

            if (model == null)
            {
                model = new RegisNotRequired_LogInModel();
            }

            return View(model);
        }


        [HttpPost]
        [ActionName(ActionNameConstants.REGIS_NOT_REQUIRED__LOG_IN_PAGE__EXECUTE_ACTION)]
        public ActionResult LogIn_ExeucteAction(RegisNotRequired_LogInModel model)
        {
            var logInSuccess = false;
            Account loggedInAccount = null;

            if (ModelState.IsValid)
            {
                var inputUsername = model.InputUsername;

                try
                {
                    var usernameExists = accountAccessor.AccountDatabaseManagerHelper.IfAccountUsernameExists(inputUsername);
                    if (usernameExists)
                    {
                        logInSuccess = accountAccessor.AccountDatabaseManagerHelper.IfAccountCanLogInWithGivenPassword(inputUsername, model.InputPassword);

                        if (logInSuccess)
                        {
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            model.StatusMessage = "Log in success"; //not to be shown anyways.
                            loggedInAccount = accountAccessor.AccountDatabaseManagerHelper.GetAccountInfoFromUsername(inputUsername);
                        }
                        else
                        {
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                            model.StatusMessage = "Log in failed. Username or password is incorrect.";
                        }
                    }
                    else
                    {
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        model.StatusMessage = "Log in failed. Username or password is incorrect.";
                    }
                }
                catch (AccountDisabledFromLoggingInException)
                {
                    logInSuccess = false;
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    model.StatusMessage = "Log in failed. This account has been disabled.";
                }
                catch (Exception)
                {
                    logInSuccess = false;
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    model.StatusMessage = "Log in failed. An error has occured.";
                }
            }

            //

            if (logInSuccess)
            {
                Session.Add(SessionConstants.SESSION_ACCOUNT_OBJ, loggedInAccount);


                var mustChangeCredentials = accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.IfBooleanCorrelationExsists(loggedInAccount.Id);
                if (mustChangeCredentials)
                {
                    return RedirectToAction(ActionNameConstants.GENERIC_USER__MUST_CHANGE_CREDENTIALS__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
                }
                else
                {
                    /*
                    if (TypeConstants.ACCOUNT_TYPE_EMPLOYEE.Equals(loggedInAccount.AccountType))
                    {
                        return RedirectToAction(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE, ControllerNameConstants.EMPLOYEE_CONTROLLER_NAME);
                    }
                    else if (TypeConstants.ACCOUNT_TYPE_ADMIN_HIGHEST_LEVEL.Equals(loggedInAccount.AccountType))
                    {
                        return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                    }
                    else if (TypeConstants.ACCOUNT_TYPE_APPLICANT.Equals(loggedInAccount.AccountType))
                    {

                    }
                    else
                    {
                        return new ContentResult(); //ERROR, Should not reach here
                    }
                    */
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
                }


            }
            else
            {
                TempData.Add(TEMP_MODEL_KEY, model);

                return RedirectToAction(ActionNameConstants.REGIS_NOT_REQUIRED__LOG_IN_PAGE, ControllerNameConstants.REGIS_NOT_REQUIRED_CONTROLLER_NAME);
            }


            //return new ContentResult();
        }

        #endregion


        //

        #region "Register"



        #endregion


    }
}