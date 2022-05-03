using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIA_Portal.Accessors;
using SIA_Portal.Constants;
using CommonDatabaseActionReusables.AccountManager;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.Models.EmployeeModel;
using SIA_Portal.Models.ObjectRepresentations;
using CommonDatabaseActionReusables.AnnouncementManager;
using SIA_Portal.Models.AdminModels;
using SIA_Portal.Controllers.BaseControllers;
using CommonDatabaseActionReusables.CategoryManager;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using SIA_Portal.Utilities.UrlResolver;
using CommonDatabaseActionReusables.ImageManager;
using System.IO;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.Models.GenericUserModel;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager;

namespace SIA_Portal.Controllers
{
    public class GenericUserController : Controller
    {
        private const string TEMP_DATA_MODEL_KEY = "TempDataModelKey";
        private const string TEMP_DATA_AD_GET_PARM_KEY = "AdGetParamKey";

        public const string TEMP_DATA_EMPLOYEE_ID_KEY = "TempDataEmoployeeIdKey";

        public const string TEMP_DATA_ANNOUNCEMENT_ID_KEY = "TempDataAnnouncementIdKey";


        private PortalAccountDivisionCategoryAccessor employeeDivCategoryAccessor = new PortalAccountDivisionCategoryAccessor();
        private PortalEmpAccToEmpRecordAccessor empAccToEmpRecordAccessor = new PortalEmpAccToEmpRecordAccessor();
        private PortalEmployeeRecordAccessor employeeRecordAccessor = new PortalEmployeeRecordAccessor();
        private PortalAccountAccessor accAccessor = new PortalAccountAccessor();

        private PortalAnnouncementAccessor annAccessor = new PortalAnnouncementAccessor();
        private PortalImageAccessor imageAccessor = new PortalImageAccessor();

        private PortalAccountMustChangeCredentialsAccessor accountChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();


        #region "Page account bar"

        public ActionResult Index()
        {
            return View();
        }

        #endregion


        #region "Employee Record Model"


        [ActionName(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO)]
        public ActionResult GoToViewEditEmployeeRecord()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ViewEditEmployeeRecordModel)TempData[TEMP_DATA_MODEL_KEY];
            //var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            var employeeId = -1;

            if (TempData.ContainsKey(TEMP_DATA_EMPLOYEE_ID_KEY))
            {
                employeeId = (int)TempData[TEMP_DATA_EMPLOYEE_ID_KEY];
            }

            return View(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO, GetConfiguredViewEmployeeRecordModel(account, employeeId, model));
        }

        private ViewEditEmployeeRecordModel GetConfiguredViewEmployeeRecordModel(Account account, int employeeId, ViewEditEmployeeRecordModel model = null)
        {
            if (model == null)
            {
                model = new ViewEditEmployeeRecordModel();
            }
            model.LoggedInAccount = account;

            //

            model.EmployeeDivCategoryList = GetConfiguredListOfEmployeeCategoriesForChoices(model.EmployeeDivCategoryList);

            if (model.EmployeeDivCategoryNameSelected == null && model.EmployeeDivCategoryList.Count > 0)
            {
                model.EmployeeDivCategoryNameSelected = model.EmployeeDivCategoryList[0];
            }

            //

            model.EmployeeId = employeeId;

            var employeeAcc = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(employeeId);
            if (employeeAcc != null)
            {
                model.EmployeeUsername = employeeAcc.Username;
            }

            if (empAccToEmpRecordAccessor.EntityToRecordDatabaseManagerHelper.IfPrimaryExsists(employeeId))
            {
                var targetIds = empAccToEmpRecordAccessor.EntityToRecordDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(employeeId, new AdvancedGetParameters());
                var recId = -1;

                foreach (int targetId in targetIds)
                {
                    recId = targetId;
                    break;
                }


                var empRecord = employeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryGetEmployeeRecordInfoFromId(recId);

                if (empRecord != null)
                {
                    model.PopulateSelfWithRecordData(empRecord, employeeDivCategoryAccessor);
                    model.RecordId = empRecord.Id;
                }
            }

            return model;
        }


        private IList<string> GetConfiguredListOfEmployeeCategoriesForChoices(IList<string> list)
        {
            if (list == null)
            {
                list = new List<string>();
            }
            list.Clear();

            var listOfCats = employeeDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());

            //list.Add(CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME);
            foreach (Category cat in listOfCats)
            {
                list.Add(cat.Name);
            }

            return list;
        }



        [ActionName(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__EXECUTE_ACTION)]
        public ActionResult ViewEditEmployeeRecord_ExecuteAction(ViewEditEmployeeRecordModel model, string executeAction)
        {
            if (executeAction.Equals("submitResponse"))
            {
                return SaveEmployeeRecordInputs(model);
            }
            else
            {
                TempData.Add(TEMP_DATA_MODEL_KEY, model);

                return RedirectToAction(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
            }
        }


        private ActionResult SaveEmployeeRecordInputs(ViewEditEmployeeRecordModel model)
        {
            if (ModelState.IsValid)
            {
                var builder = new EmployeeRecord.Builder();

                model.PopulateBuilder(builder, employeeDivCategoryAccessor);

                if (model.RecordId == -1) //Create, since record is not created yet.
                {
                    var recId = -1;

                    try
                    {
                        recId = employeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryCreateEmployeeRecord(builder);
                    }
                    catch (InputStringConstraintsViolatedException e)
                    {
                        model.StatusMessage = string.Format("Invalid character detected: {0}", e.ViolatingString);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (Exception)
                    {
                        model.StatusMessage = "Record creation failed! A generic error has occurred.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }

                    model.RecordId = recId;

                    //

                    if (recId != -1)
                    {
                        var success = false;

                        try
                        {
                            success = empAccToEmpRecordAccessor.EntityToRecordDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.EmployeeId, recId);
                        }
                        catch (Exception)
                        {
                            model.StatusMessage = "Record creation failed! A generic error has occurred. (Err Code: IntermediaryA)";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                        if (success)
                        {
                            model.StatusMessage = "Record created!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(model.StatusMessage))
                            {
                                model.StatusMessage = "Record creation failed! (Err Code: IntermediaryX)";
                            }
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;

                            employeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.DeleteEmployeeRecordWithId(recId);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.StatusMessage))
                        {
                            model.StatusMessage = "Record creation failed!";
                        }
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }

                }
                else //Edit, since record exists.
                {
                    var success = false;

                    try
                    {
                        success = employeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.EditEmployeeRecord(model.RecordId, builder);
                    }
                    catch (InputStringConstraintsViolatedException e)
                    {
                        model.StatusMessage = string.Format("Invalid character detected: {0}", e.ViolatingString);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (Exception e) 
                    {
                        model.StatusMessage = string.Format("A generic error has occured.");
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }

                    if (success)
                    {
                        model.StatusMessage = "Record edited!";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.StatusMessage))
                        {
                            model.StatusMessage = "Record edit failed!";
                        }
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.StatusMessage))
                {
                    model.StatusMessage = "Record edit failed! Invalid inputs detected!";
                }
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_EMPLOYEE_ID_KEY, model.EmployeeId);


            return RedirectToAction(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);

        }




        #endregion


        #region "Announcement"


        [ActionName(ActionNameConstants.GENERIC_USER__ANNOUNCEMENT_PAGE__GO_TO)]
        public ActionResult ViewAnnouncement()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ViewAnnouncementModel)TempData[TEMP_DATA_MODEL_KEY];
            var annId = (int)TempData[TEMP_DATA_ANNOUNCEMENT_ID_KEY];

            return View(GetConfiguredAnnouncementModel(account, model, annId));
        }


        private ViewAnnouncementModel GetConfiguredAnnouncementModel(Account account, ViewAnnouncementModel model, int announcementId)
        {
            if (model == null)
            {
                model = new ViewAnnouncementModel();
            }
            model.LoggedInAccount = account;


            var ann = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(announcementId);
            model.AnnouncementId = announcementId;
            if (ann != null)
            {
                model.AnnouncementTitle = ann.Title;
                model.AnnouncementContent = ann.GetContentAsString();

                var imageId = ann.MainImageId;
                string imagePathOrContent = null;
                var image = imageAccessor.ImageDatabaseManagerHelper.TryGetImageContentInfoFromId(imageId);
                if (image != null)
                {
                    if (image.StoreType == ImageStoreType.TYPE_DIRECTORY_PATH)
                    {
                        imagePathOrContent = UrlResolver.RelativePath(image.ImageFullPath, System.Web.HttpContext.Current.Request);
                    }
                    else
                    {
                        imagePathOrContent = image.GetImageBytesAsString();
                    }

                    model.AnnouncementImagePathOrContent = imagePathOrContent;
                }

            }

            return model;
        }


        #endregion


        #region "Change Acc Password"


        [ActionName(ActionNameConstants.GENERIC_USER__CHANGE_OWN_PASSWORD__GO_TO)]
        public ActionResult GoToChangeOwnSettingsPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];

            return View(new ChangeOwnAccountSettingsModel(account));
        }


        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_USER__CHANGE_OWN_PASSWORD__GO_TO)]
        public ActionResult ChangeOwnSettings_FromChangeSettingsPage(ChangeOwnAccountSettingsModel model)
        {
            model.LoggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];

            if (ModelState.IsValid)
            {
                //var account = (Account)System.Web.HttpContext.Current.Session[ControllerConstants.SESSION_ACCOUNT_OBJ];
                bool editSuccess;

                try
                {
                    var currPasswordsMatch = accAccessor.AccountDatabaseManagerHelper.AccountPasswordMatchesWithGiven(model.LoggedInAccount.Id, model.InputCurrentPassword);

                    if (currPasswordsMatch)
                    {
                        if (model.IfNewAndConfirmPasswordsAreEqual())
                        {
                            editSuccess = accAccessor.AccountDatabaseManagerHelper.EditAccountWithId(model.LoggedInAccount.Id, null, model.InputNewPassword);

                            if (editSuccess)
                            {
                                model.StatusMessage = "Changes saved!";
                                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_SUCCESS;
                            }
                            else
                            {
                                model.StatusMessage = "An error has occured. Please try again.";
                                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                            }
                        }
                        else
                        {
                            editSuccess = false;
                            model.StatusMessage = "New and Confirm passwords do not match";
                            model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                        }
                    }
                    else
                    {
                        editSuccess = false;
                        model.StatusMessage = "Current password is incorrect";
                        model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                    }


                }
                catch (SqlException)
                {
                    model.StatusMessage = "An error has occured. Please try again.";
                    model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                }


                return View(model);
            }
            else
            {
                model.StatusMessage = String.Format("Invalid inputs detected");
                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;

                return View(model);
            }


        }

        #endregion


        #region "Change Acc Credentials"


        [ActionName(ActionNameConstants.GENERIC_USER__MUST_CHANGE_CREDENTIALS__GO_TO)]
        public ActionResult GoToChangeOwnCredentialsPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];

            return View(new ChangeOwnCredentialsModel(account));
        }


        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_USER__MUST_CHANGE_CREDENTIALS__GO_TO)]
        public ActionResult ChangeOwnCredentials_FromChangeCredeitnalsPage(ChangeOwnCredentialsModel model)
        {
            model.LoggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];

            if (ModelState.IsValid)
            {
                bool editSuccess;

                try
                {
                    var ifUsernameExists = accAccessor.AccountDatabaseManagerHelper.IfAccountUsernameExists(model.InputNewUsername);

                    if (!ifUsernameExists)
                    {
                        if (model.IfNewAndConfirmPasswordsAreEqual())
                        {
                            var accBuilder = accAccessor.AccountDatabaseManagerHelper.GetAccountInfoFromId(model.LoggedInAccount.Id).ConstructBuilderUsingSelf();
                            accBuilder.Username = model.InputNewUsername;

                            editSuccess = accAccessor.AccountDatabaseManagerHelper.EditAccountWithId(model.LoggedInAccount.Id, accBuilder, model.InputNewPassword);


                            if (editSuccess)
                            {
                                model.StatusMessage = "Changes saved!";
                                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_SUCCESS;

                                //
                                RemoveAccountFromMustChange(model.LoggedInAccount.Id);
                                return LogOutFromSite(model.LoggedInAccount);
                            }
                            else
                            {
                                model.StatusMessage = "An error has occured. Please try again.";
                                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                            }
                        }
                        else
                        {
                            editSuccess = false;
                            model.StatusMessage = "New and Confirm passwords do not match";
                            model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                        }
                    }
                    else
                    {
                        editSuccess = false;
                        model.StatusMessage = "Username is already taken";
                        model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                    }


                }
                catch (SqlException)
                {
                    model.StatusMessage = "An error has occured. Please try again.";
                    model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;
                }


                return View(model);
            }
            else
            {
                model.StatusMessage = String.Format("Invalid inputs detected");
                model.ChangeOwnSettingsStatus = ActionStatusConstants.STATUS_FAILED;

                return View(model);
            }


        }


        private void RemoveAccountFromMustChange(int accId)
        {
            var exists = accountChangeCredentialsAccessor.BooleanDatabaseManagerHelper.IfBooleanCorrelationExsists(accId);

            if (exists)
            {
                accountChangeCredentialsAccessor.BooleanDatabaseManagerHelper.DeleteBooleanCorrWithId(accId);
            }
        }

        private ActionResult LogOutFromSite(Account loggedInAccount)
        {
            SessionConstants.RemoveSessionObjectsFromSession(System.Web.HttpContext.Current);

            return RedirectToAction(ActionNameConstants.REGIS_NOT_REQUIRED__LOG_IN_PAGE, ControllerNameConstants.REGIS_NOT_REQUIRED_CONTROLLER_NAME);
        }

        #endregion

    }
}