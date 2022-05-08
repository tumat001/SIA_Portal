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
using SIA_Portal.CustomAccessors;
using SIA_Portal.CustomAccessors.RequestableDocument;
using CommonDatabaseActionReusables.QueueManager;
using CommonDatabaseActionReusables.NotificationManager;
using CommonDatabaseActionReusables.DocumentManager;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Controllers
{
    public class GenericUserController : Controller
    {
        private const string TEMP_DATA_MODEL_KEY = "TempDataModelKey";
        private const string TEMP_DATA_AD_GET_PARM_KEY = "AdGetParamKey";

        public const string TEMP_DATA_EMPLOYEE_ID_KEY = "TempDataEmoployeeIdKey";

        public const string TEMP_DATA_ANNOUNCEMENT_ID_KEY = "TempDataAnnouncementIdKey";

        public const string TEMP_DATA_READ_NOTIF_KEY = "TempDataReadNotifKey";


        private PortalAccountDivisionCategoryAccessor employeeDivCategoryAccessor = new PortalAccountDivisionCategoryAccessor();
        private PortalEmpAccToEmpRecordAccessor empAccToEmpRecordAccessor = new PortalEmpAccToEmpRecordAccessor();
        private PortalEmployeeRecordAccessor employeeRecordAccessor = new PortalEmployeeRecordAccessor();
        private PortalAccountAccessor accAccessor = new PortalAccountAccessor();

        private PortalAnnouncementAccessor annAccessor = new PortalAnnouncementAccessor();
        private PortalImageAccessor imageAccessor = new PortalImageAccessor();

        private PortalAccountMustChangeCredentialsAccessor accountChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();

        private PortalRequestDocuAccessor requestDocuAccessor = new PortalRequestDocuAccessor();
        private PortalReqDocuQueueAccessor reqDocuQueueAccessor = new PortalReqDocuQueueAccessor();
        private PortalReqDocuQueueToAccountAccessor reqDocuQueueToAccountAccessor = new PortalReqDocuQueueToAccountAccessor();
        private PortalReqDocuQueueToReqDocuAccessor reqDocuQueueToReqDocuAccessor = new PortalReqDocuQueueToReqDocuAccessor();

        private PortalNotificationAccessor notificationAccessor = new PortalNotificationAccessor();
        private PortalAccountToNotificationRelationAccessor accountToNotificationAccessor = new PortalAccountToNotificationRelationAccessor();
        private PortalNotificationToDocumentRelationAccessor notificationToDocumentAccessor = new PortalNotificationToDocumentRelationAccessor();

        private PortalDocumentFileAccessor documentFileAccessor = new PortalDocumentFileAccessor();



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


        #region "Requesting for Docu"

        [HttpGet]
        [ActionName(ActionNameConstants.GENERIC_USER__REQUEST_DOCUMENT_ID_SELECTED)]
        public JsonResult GetRequestableDocumentRepresentationFromId(string docuId)
        {
            var id = int.Parse(docuId);

            var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(id);
            var rep = new RequestableDocumentRepresentation();

            if (docu != null)
            {
                rep.Title = docu.DocumentName;
                rep.FullDescription = docu.GetNoteDescriptionAsString();
                rep.Id = docu.Id;
            }

            return Json(rep, JsonRequestBehavior.AllowGet);
        }



        [ActionName(ActionNameConstants.GENERIC_USER__REQUEST_FOR_DOCUMENT__GO_TO)]
        public ActionResult GoTo_RequestForDocumentPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (RequestForDocumentModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfigured_RequestableDocumentsModel(model, account));
        }

        
        private RequestForDocumentModel GetConfigured_RequestableDocumentsModel(RequestForDocumentModel model, Account account)
        {
            if (model == null)
            {
                model = new RequestForDocumentModel();
            }
            model.LoggedInAccount = account;


            var allDocus = requestDocuAccessor.ReqDocuManagerHelper.TryAdvancedGetRequestableDocumentsAsList(new AdvancedGetParameters());
            if (model.AllRequestableDocumentRepresentations == null)
            {
                model.AllRequestableDocumentRepresentations = new List<RequestableDocumentRepresentation>();
            }

            model.AllRequestableDocumentRepresentations.Clear();
            foreach (RequestableDocument docu in allDocus)
            {
                var rep = new RequestableDocumentRepresentation();
                rep.Title = docu.DocumentName;
                rep.FullDescription = docu.GetNoteDescriptionAsString();
                rep.Id = docu.Id;

                model.AllRequestableDocumentRepresentations.Add(rep);
            }

            if (model.SelectedDocumentRepresentation == null && model.AllRequestableDocumentRepresentations.Count > 1)
            {
                model.SelectedDocumentRepresentation = model.AllRequestableDocumentRepresentations[0];
                model.SelectedDocumentRepId = model.SelectedDocumentRepresentation.Id;
            }


            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_USER__REQUEST_FOR_DOCUMENT__EXECUTE_ACTION)]
        public ActionResult RequestForDocumentPage_ExecuteAction(RequestForDocumentModel model, string executeAction)
        {
            if (executeAction.Equals("RequestAction"))
            {
                return CreateRequestForDocument_AndSendToQueue(model);
            }
            else
            {
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
            }
        }


        private ActionResult CreateRequestForDocument_AndSendToQueue(RequestForDocumentModel model)
        {
            if (ModelState.IsValid)
            {
                
                var createSuccess = AttemptCreateRequestQueue_AndAssociatedActions(model);

                if (createSuccess)
                {
                    model.StatusMessage = "Request for document sent!";
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;

                }
                else
                {
                    model.StatusMessage = "Error in sending request. Please try again.";
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }


            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__REQUEST_FOR_DOCUMENT__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }


        private bool AttemptCreateRequestQueue_AndAssociatedActions(RequestForDocumentModel model)
        {
            var success = false;

            try
            {
                var builder = new Queue.Builder();
                builder.SetDescriptionUsingString(model.InputReasonForRequest);
                builder.PriorityLevel = model.InputPriorityLevel;
                builder.Status = QueueStatusConstants.STATUS_PENDING;
                builder.SetStatusDescriptionUsingString("");
                builder.DateTimeOfQueue = DateTime.Now;

                var queueId = reqDocuQueueAccessor.QueueDatabaseManagerHelper.CreateQueue(builder);

                if (queueId != -1)
                {
                    var loggedInAcc = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
                    var successCreateDocuToAcc = reqDocuQueueToAccountAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(queueId, loggedInAcc.Id);

                    //
                    var docuSelected = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(model.SelectedDocumentRepId);

                    if (docuSelected != null)
                    {
                        var successCreateQueueToDocu = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(queueId, docuSelected.Id);

                        success = successCreateDocuToAcc & successCreateQueueToDocu;
                    }

                }
            }
            catch (Exception)
            {

            }

            return success;
        }


        #endregion


        //


        #region "Manage Notification Table page"

        [ActionName(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO)]
        public ActionResult GoToManageNotificationPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageNotificationModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO, GetConfiguredManageNotificationModel(account, adGetParam, model));
        }

        private ManageNotificationModel GetConfiguredManageNotificationModel(Account account, AdvancedGetParameters adGetParam = null, ManageNotificationModel model = null)
        {
            if (model == null)
            {
                model = new ManageNotificationModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_NOTIFICATIONS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (NotificationRepresentation rep in model.EntityRepresentationsInPage)
                {
                    if (rep.IsSelected)
                    {
                        if (!selectedIds.Contains(rep.Id))
                        {
                            selectedIds.Add(rep.Id);
                        }
                    }
                    else
                    {
                        if (selectedIds.Contains(rep.Id))
                        {
                            selectedIds.Remove(rep.Id);
                        }
                    }

                }
            }

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_NOTIFICATIONS] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageNotificationModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageNotificationModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            var notifIdsOfLoggedIn = accountToNotificationAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.LoggedInAccount.Id, adGetParam);
            var entities = new List<Notification>();
            foreach (int id in notifIdsOfLoggedIn)
            {
                entities.Add(notificationAccessor.NotificationManagerHelper.TryGetNotificationInfoFromId(id));
            }



            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<NotificationRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Notification ent in entities)
            {
                var rep = new NotificationRepresentation();
                rep.Id = ent.Id;
                rep.Title = ent.Title;
                rep.ContentPreview = ent.GetContentAsString();
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            //model.TotalEntityCount = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementCount(countAdGetParam);
            var notifIdCountOfLoggedInAcc = accountToNotificationAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationOfPrimaryCount(model.LoggedInAccount.Id, countAdGetParam);
            model.TotalEntityCount = notifIdCountOfLoggedInAcc;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO)]
        public ActionResult ManageNotificationPage_ExecuteAction(ManageNotificationModel model, string executeAction)
        {
            if (executeAction.Equals("ReadAction"))
            {
                return TransitionFromNotificationManagePage_ToReadNotificationPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                return DeleteSelectedNotifications(model);
                //return TransitionFromManagePage_ToDeleteAnnouncementPage(model);
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageNotificationModel_ThenGoToManagePage(model);
            }
            else
            {
                return ManageNotificationPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageNotificationPage_DoPageIndexChange(ManageNotificationModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }


        private ActionResult SetTextFilter_ToManageNotificationModel_ThenGoToManagePage(ManageNotificationModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_NOTIFICATIONS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }


        private ActionResult TransitionFromNotificationManagePage_ToReadNotificationPage(ManageNotificationModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageNotificationModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_NOTIFICATIONS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Notification notif = notificationAccessor.NotificationManagerHelper.TryGetNotificationInfoFromId(selectedIds[0]);

                if (notif != null)
                {
                    TempData.Add(TEMP_DATA_READ_NOTIF_KEY, notif);

                    return RedirectToAction(ActionNameConstants.GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }



        private ActionResult DeleteSelectedNotifications(ManageNotificationModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageNotificationModel(loggedInAccount, null, model);


            var idsToRemove = new List<int>();

            var adGetParam = new AdvancedGetParameters();
            foreach (int id in model.SelectedEntityIds)
            {
                var docuIdsSet = notificationToDocumentAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(id, adGetParam);
                
                //

                var success = notificationAccessor.NotificationManagerHelper.TryDeleteNotificationWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }

                //

                foreach (int docuId in docuIdsSet)
                {
                    if (!notificationToDocumentAccessor.EntityToCategoryDatabaseManagerHelper.IfTargetExists(docuId))
                    {
                        documentFileAccessor.DocumentDatabaseManagerHelper.DeleteDocumentWithId(docuId);
                    }
                }

            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            return RedirectToAction(ActionNameConstants.GENERIC_USER__MANAGE_NOTIFICATION_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }



        #endregion

        #region "View Notification"

        [ActionName(ActionNameConstants.GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__GO_TO)]
        public ActionResult GoToViewSingleNotificationPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ViewNotificationModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__GO_TO, GetConfiguredViewNotificationModel(account, adGetParam, model));
        }

        private ViewNotificationModel GetConfiguredViewNotificationModel(Account account, AdvancedGetParameters adGetParam = null, ViewNotificationModel model = null)
        {
            if (model == null)
            {
                model = new ViewNotificationModel(account);
            }
            model.LoggedInAccount = account;

            //

            var tempSelectedNotif = (Notification)TempData[TEMP_DATA_READ_NOTIF_KEY];
            if (tempSelectedNotif != null)
            {
                var rep = new NotificationRepresentation();
                rep.Id = tempSelectedNotif.Id;
                rep.Title = tempSelectedNotif.Title;
                rep.FullContent = tempSelectedNotif.GetContentAsString();

                model.NotifRep = rep;
            }

            //

            if (model.NotifRep != null )//&& model.DocuFileRep == null)
            {
                if (notificationToDocumentAccessor.EntityToCategoryDatabaseManagerHelper.TryIfPrimaryExists(model.NotifRep.Id))
                {
                    
                    var docuId = -1;
                    var docuSet = notificationToDocumentAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(model.NotifRep.Id, new AdvancedGetParameters());
                    foreach (int id in docuSet)
                    {
                        docuId = id;
                    }

                    var docu = documentFileAccessor.DocumentDatabaseManagerHelper.GetDocumentInfoFromId(docuId);
                    if (docu != null)
                    {
                        model.HasDocu = true;

                        /*
                        var rep = new DocumentFileRepresentation();
                        rep.Id = docu.Id;
                        rep.OriginalFileName = docu.OriginalNameOfFile;
                        rep.DocuExt = docu.DocumentExtension;
                        rep.DocuBytes = docu.DocumentContentAsBytes;
                        rep.DirectoryPath = docu.FullPath;

                        model.DocuFileRep = rep;
                        */
                        model.DocuId = docu.Id;
                        model.DocuOriginalName = docu.OriginalNameOfFile;
                    }

                }
            }

            //

            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.GENERIC_USER__VIEW_SINGLE_NOTIFICATION_PAGE__EXECUTE_ACTION)]
        public ActionResult ViewNotificationPage_ExecuteAction(ViewNotificationModel model, string executeAction)
        {
            return DownloadDocumentFile(int.Parse(executeAction));
        }


        [HttpGet]
        public ActionResult DownloadDocumentFile(int docuId)
        {
            var docuFile = documentFileAccessor.DocumentDatabaseManagerHelper.GetDocumentInfoFromId(docuId);

            string path = docuFile.FullPath;
            string filename2 = docuFile.OriginalNameOfFile;
            byte[] filebytes = docuFile.DocumentContentAsBytes;
            string contentType = MimeMapping.GetMimeMapping(filename2 + docuFile.DocumentExtension);


            return File(filebytes, contentType, filename2 + docuFile.DocumentExtension);
        }


        #endregion


    }
}