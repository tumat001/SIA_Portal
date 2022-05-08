using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIA_Portal.Accessors;
using SIA_Portal.Constants;
using SIA_Portal.Models.RegisNotRequired;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Controllers;
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
using SIA_Portal.Utilities.DomIdentifier;
using CommonDatabaseActionReusables.AccountManager.Actions.Exceptions;
using CommonDatabaseActionReusables.BooleanCorrManager;
using CommonDatabaseActionReusables.PermissionsManager;
using SIA_Portal.CustomAccessors;
using SIA_Portal.CustomAccessors.RequestableDocument;
using CommonDatabaseActionReusables.RelationManager;
using CommonDatabaseActionReusables.QueueManager;
using CommonDatabaseActionReusables.NotificationManager;
using System.Text;
using CommonDatabaseActionReusables.DocumentManager;

namespace SIA_Portal.Controllers
{
    public class AdminController : BaseControllerWithNavBarController
    {

        private const string TEMP_DATA_MODEL_KEY = "TempDataModelKey";
        private const string TEMP_DATA_AD_GET_PARM_KEY = "AdGetParamKey";
        private const string TEMP_DATA_DELETE_AD_GET_PARAM_KEY = "Delete_AdGetParamKey";

        public const string TEMP_DATA_CONFIRM_DELETE_MODEL_KEY = "TempDataConfirmDeleteModelKey";

        public const string TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY = "AccountToEdit";
        public const string TEMP_DATA_EDIT_ANNOUNCEMENT_KEY = "AnnouncementToEdit";

        public const string TEMP_DATA_EDIT_PERMISSION_KEY = "PermissionToEdit";
        public const string TEMP_DATA_EDIT_REQUESTABLE_DOCU_KEY = "RequestableDocuToEdit";
        public const string TEMP_DATA_EDIT_ACCOUNT_DIVISION_KEY = "AccountDivisionToEdit";

        public const string TEMP_DATA_EDIT_PERMISSION_OF_ACCOUNT_KEY = "AccountPermissionOfAccountToEdit";
        public const string TEMP_DATA_FULFILL_ACKNOWLEDGED_REQ_DOCU_KEY = "AcknowledgedReqDocuToFulfill";
        public const string TEMP_DATA_TERMINATE_ACKNOWLEDGED_REQ_DOCU_KEY = "AcknowledgedReqDocuToTerminate";

        //

        public const string DOM_IDENTIFIER_TYPE_SINGLE_ANNOUNCEMENT = "SingleAnnouncement";
        public const string DOM_IDENFIFIER_TYPE_ANNOUNCEMENT_PAGE_IN_INDEX = "AnnPageInIndex";

        private const string ANNOUNCE_TO_ALL_ITEM = "(All)";

        //


        private PortalAnnouncementAccessor annAccessor = new PortalAnnouncementAccessor();
        private PortalAccountAccessor accAccessor = new PortalAccountAccessor();

        private PortalAccountDivisionCategoryAccessor accDivCategoryAccessor = new PortalAccountDivisionCategoryAccessor();
        private PortalAccountToAccDivCatAccessor accToDivCatAccessor = new PortalAccountToAccDivCatAccessor();
        private PortalImageAccessor imageAccessor = new PortalImageAccessor();

        //private PortalAnnouncementCategoryAccessor annCategoryAccessor = new PortalAnnouncementCategoryAccessor();
        private PortalAnnToCatAccessor annToCatAccessor = new PortalAnnToCatAccessor();

        private PortalEmpAccToEmpRecordAccessor empAccToEmpRecordAccessor = new PortalEmpAccToEmpRecordAccessor();
        private PortalEmployeeRecordAccessor employeeRecordAccessor = new PortalEmployeeRecordAccessor();

        private PortalAccountMustChangeCredentialsAccessor accMustChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();

        private PortalPermissionAccessor permissionAccessor = new PortalPermissionAccessor();
        private PortalRequestDocuAccessor requestDocuAccessor = new PortalRequestDocuAccessor();

        private PortalAccountToDivisionResponsibilityAccessor accToDivRespoAccessor = new PortalAccountToDivisionResponsibilityAccessor();
        private PortalAccountToPermissionsAccessor accountToPermissionsAccessor = new PortalAccountToPermissionsAccessor();

        private PortalReqDocuQueueAccessor reqDocuQueueAccessor = new PortalReqDocuQueueAccessor();
        private PortalReqDocuQueueToReqDocuAccessor reqDocuQueueToReqDocuAccessor = new PortalReqDocuQueueToReqDocuAccessor();
        private PortalReqDocuQueueToAccountAccessor reqDocuQueueToAccAccessor = new PortalReqDocuQueueToAccountAccessor();

        private PortalFulfillerAccToReqDocuQueueAccessor fulfillerAccToReqDocuQueueAccessor = new PortalFulfillerAccToReqDocuQueueAccessor();

        private PortalNotificationAccessor notificationAccessor = new PortalNotificationAccessor();
        private PortalAccountToNotificationRelationAccessor accountToNotificationAccessor = new PortalAccountToNotificationRelationAccessor();
        private PortalNotificationToDocumentRelationAccessor notificationToDocumentAccessor = new PortalNotificationToDocumentRelationAccessor();

        private PortalDocumentFileAccessor documentFileAccessor = new PortalDocumentFileAccessor();


        //Used in bool function for lambda in if account has a divisional category within curr logged in acc's div respo.
        private Account currentLoggedInAccount;
        private AdvancedGetParameters currentAdGetParam;
        private QueueAdvancedGetParameters currentQueueAdGetParam;
        //

        #region "funcs of logged in acc's categories"


        public bool IsAccountInAllDivisionResponsibilityOfLoggedIn(int accId, int divId)
        {
            var result = false;

            var account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(accId);
            if (account != null)
            {
                var divIdsOfAccount = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(accId, new AdvancedGetParameters());
                var divRespoIdOfLoggedInAcc = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(currentLoggedInAccount.Id, new AdvancedGetParameters());

                if (divIdsOfAccount.Count > divRespoIdOfLoggedInAcc.Count)
                {
                    return false;
                }

                var hasAllRelevantRespo = true;

                foreach (int id in divIdsOfAccount)
                {
                    if (!divRespoIdOfLoggedInAcc.Contains(id))
                    {
                        hasAllRelevantRespo = false;
                    }
                }

                //

                var textFilterPassed = String.IsNullOrEmpty(currentAdGetParam.TextToContain) || account.Username.Contains(currentAdGetParam.TextToContain);

                //

                result = hasAllRelevantRespo & textFilterPassed;


            }

            return result;
        }



        public bool IsAnnouncementInAtLeastOneDivisionResponsibilityOfLoggedIn(int annId, int divId)
        {
            var result = false;

            var announcement = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(annId);
            if (announcement != null)
            {

                var divIdsOfAnnouncement = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(annId, new AdvancedGetParameters());
                var divRespoIdOfLoggedInAcc = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(currentLoggedInAccount.Id, new AdvancedGetParameters());

                var hasOneRelevantRespo = false;

                foreach (int id in divIdsOfAnnouncement)
                {
                    if (divRespoIdOfLoggedInAcc.Contains(id))
                    {
                        hasOneRelevantRespo = true;
                        break;
                    }
                }

                //

                var textFilterPassed = String.IsNullOrEmpty(currentAdGetParam.TextToContain) || announcement.Title.Contains(currentAdGetParam.TextToContain);

                //

                result = hasOneRelevantRespo & textFilterPassed;

            }

            return result;
        }



        public bool IsAnnouncementInAllDivisionResponsibilityOfLoggedIn(int annId, int divId)
        {
            var result = false;

            var announcement = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(annId);
            if (announcement != null)
            {

                
                var divIdsOfAnnouncement = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(annId, new AdvancedGetParameters());
                var divRespoIdOfLoggedInAcc = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(currentLoggedInAccount.Id, new AdvancedGetParameters());

                if (divIdsOfAnnouncement.Count > divRespoIdOfLoggedInAcc.Count)
                {
                    return false;
                }

                var hasAllRelevantRespo = true;

                foreach (int id in divIdsOfAnnouncement)
                {
                    if (!divRespoIdOfLoggedInAcc.Contains(id))
                    {
                        hasAllRelevantRespo = false;
                    }
                }

                //

                var textFilterPassed = String.IsNullOrEmpty(currentAdGetParam.TextToContain) || announcement.Title.Contains(currentAdGetParam.TextToContain);

                //

                result = hasAllRelevantRespo & textFilterPassed;
                
            }

            return result;
        }


        public IList<Category> GetDivisionalResponsibilitiesOfAccount(Account acc, AdvancedGetParameters adGetParam)
        {
            var bucket = new List<Category>();

            var idOfDivRes = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(acc.Id, adGetParam);
            foreach (int id in idOfDivRes)
            {
                var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromId(id);

                if (cat != null)
                {
                    bucket.Add(cat);
                }
            }

            return bucket;
        }



        public bool IsQueueAssociatedAccInAllDivisionResponsibilityOfLoggedIn_AndSatisfyQueueAdGetParam(int queueId, int accId)
        {
            var result = false;

            var account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(accId);
            if (account != null)
            {
                var divIdsOfAccount = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(accId, new AdvancedGetParameters());
                var divRespoIdOfLoggedInAcc = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(currentLoggedInAccount.Id, new AdvancedGetParameters());

                if (divIdsOfAccount.Count > divRespoIdOfLoggedInAcc.Count)
                {
                    return false;
                }

                var hasAllRelevantRespo = true;

                foreach (int id in divIdsOfAccount)
                {
                    if (!divRespoIdOfLoggedInAcc.Contains(id))
                    {
                        hasAllRelevantRespo = false;
                    }
                }

                //

                var textFilterPassed = String.IsNullOrEmpty(currentAdGetParam.TextToContain) || account.Username.Contains(currentAdGetParam.TextToContain);

                //

                var queueAdGetParamPassed = true;
                var queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.GetQueueInfoFromId(queueId);

                if (currentQueueAdGetParam != null && queue != null)
                {
                    queueAdGetParamPassed = queue.Status == currentQueueAdGetParam.StatusFilter;
                }

                //

                result = hasAllRelevantRespo & textFilterPassed & queueAdGetParamPassed;


            }

            return result;
        }



        #endregion


        #region "Home Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__HOME_PAGE)]
        public ActionResult Index()
        {
            //var account = TempData[ControllerConstants.TEMP_ACCOUNT_OBJ];
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];
            var model = (AdminHomePageModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfiguredAdminHomePageModel(account, model, adGetParam));
        }


        private AdminHomePageModel GetConfiguredAdminHomePageModel(Account account, AdminHomePageModel model, AdvancedGetParameters adGetParam)
        {
            if (model == null)
            {
                model = new AdminHomePageModel();
            }
            model.LoggedInAccount = account;


            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = EmployeeHomePageModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = EmployeeHomePageModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.AnnouncementTitleFilter;

            model.AdGetParam = adGetParam;


            //var announcements = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);
            currentAdGetParam = adGetParam;
            currentLoggedInAccount = model.LoggedInAccount;

            var annIdsInDivRespoOfLoggedIn = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedRelationsSatisfyingCondition(adGetParam,
                IsAnnouncementInAtLeastOneDivisionResponsibilityOfLoggedIn,
                false
                );
            var announcements = new List<Announcement>();
            foreach (Relation rel in annIdsInDivRespoOfLoggedIn)
            {
                announcements.Add(annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(rel.PrimaryId));
            }

            //

            model.EntitiesInPage = announcements;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<AnnouncementRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Announcement ann in announcements)
            {
                var annRep = new AnnouncementRepresentation();
                annRep.Id = ann.Id;
                annRep.Title = ann.Title;
                annRep.ContentPreview = EmployeeHomePageModel.GetAnnouncementPreview(ann);

                model.EntityRepresentationsInPage.Add(annRep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;


            //model.TotalEntityCount = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementCount(countAdGetParam);
            currentAdGetParam = countAdGetParam;
            var allAnnIdCountInDivRespoOfLoggedIn = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationCountSatisfyingCondition(countAdGetParam,
                IsAnnouncementInAtLeastOneDivisionResponsibilityOfLoggedIn,
                false
                );
            model.TotalEntityCount = allAnnIdCountInDivRespoOfLoggedIn;


            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            //

            model.LoggedInAccountHasAcknowledgedReqDocuQueue = fulfillerAccToReqDocuQueueAccessor.EntityToCategoryDatabaseManagerHelper.TryIfPrimaryExists(model.LoggedInAccount.Id);
            //model.LoggedInAccountHasAcknowledgedReqDocuQueue = true;

            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__HOME_PAGE__EXECUTE_ACTION)]
        public ActionResult IndexPage_ExecuteAction(AdminHomePageModel model, string executeAction)
        {
            if (DomIdentifier.IsDomIdentifier(executeAction))
            {
                var domElements = DomIdentifier.GetElementsOfDomIdentifier(executeAction);

                if (domElements[0].Equals(DOM_IDENTIFIER_TYPE_SINGLE_ANNOUNCEMENT))
                {
                    var annId = int.Parse(domElements[1]);

                    return AdminHomePage_TransitionToSingleAnnouncementPage(annId);
                }
                else if (domElements[0].Equals(DOM_IDENFIFIER_TYPE_ANNOUNCEMENT_PAGE_IN_INDEX))
                {
                    var pageIndex = int.Parse(domElements[1]);

                    return AdminHomePage_DoPageIndexChange_OfAnnouncement(model, pageIndex);
                }
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_OfHomePageModelModel_ThenGoToHomePage(model);
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, "Admin");
        }


        private ActionResult AdminHomePage_DoPageIndexChange_OfAnnouncement(AdminHomePageModel model, int pageIndex)
        {
            model.CurrentPageIndex = pageIndex;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, "Admin");
        }

        private ActionResult AdminHomePage_TransitionToSingleAnnouncementPage(int annId)
        {
            TempData.Add(GenericUserController.TEMP_DATA_ANNOUNCEMENT_ID_KEY, annId);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__ANNOUNCEMENT_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }

        private ActionResult SetTextFilter_OfHomePageModelModel_ThenGoToHomePage(AdminHomePageModel model)
        {
            model.CurrentPageIndex = 1;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__HOME_PAGE, "Admin");
        }

        #endregion



        // NOTE: admin acc = (just normal) acc

        #region "Manage Admin Acc Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO)]
        public ActionResult GoToManageAdminPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageAdminAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, GetConfiguredManageAdminAccountsModel(account, adGetParam, model));
        }

        private ManageAdminAccountsModel GetConfiguredManageAdminAccountsModel(Account account, AdvancedGetParameters adGetParam = null, ManageAdminAccountsModel model = null)
        {
            if (model == null)
            {
                model = new ManageAdminAccountsModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedAdminIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];
            if (selectedAdminIds == null)
            {
                selectedAdminIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (AdminAccountRepresentation accRep in model.EntityRepresentationsInPage)
                {
                    if (accRep.IsSelected)
                    {
                        if (!selectedAdminIds.Contains(accRep.Id))
                        {
                            selectedAdminIds.Add(accRep.Id);
                        }
                    }
                    else
                    {
                        if (selectedAdminIds.Contains(accRep.Id))
                        {
                            selectedAdminIds.Remove(accRep.Id);
                        }
                    }

                }
            }

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS] = selectedAdminIds;
            model.SelectedEntityIds = selectedAdminIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAdminAccountsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageAdminAccountsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.AccountUsernameFilter;

            model.AdGetParam = adGetParam;

            //

            //var adminAccs = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountsAsList(adGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);
            //CHANGE 1 START
            currentAdGetParam = adGetParam;
            currentLoggedInAccount = model.LoggedInAccount;

            var accIdsInDivRespoOfLoggedIn = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedRelationsSatisfyingCondition(adGetParam,
                IsAccountInAllDivisionResponsibilityOfLoggedIn,
                false
                );
            var adminAccs = new List<Account>();
            foreach (Relation rel in accIdsInDivRespoOfLoggedIn)
            {
                adminAccs.Add(accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(rel.PrimaryId));
            }
            //CHANGE 1 END

            model.EntitiesInPage = adminAccs;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<AdminAccountRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Account acc in adminAccs)
            {
                var accRep = new AdminAccountRepresentation();
                accRep.Id = acc.Id;
                accRep.Username = acc.Username;
                accRep.DisabledFromLogIn = acc.DisabledFromLogIn;
                accRep.IsSelected = model.SelectedEntityIds.Contains(acc.Id);

                model.EntityRepresentationsInPage.Add(accRep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;


            //model.TotalEntityCount = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountCount(countAdGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);

            //CHANGE 2 START
            currentAdGetParam = countAdGetParam;
            var allAccIdCountInDivRespoOfLoggedIn = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationCountSatisfyingCondition(countAdGetParam,
                IsAccountInAllDivisionResponsibilityOfLoggedIn,
                false
                );
            model.TotalEntityCount = allAccIdCountInDivRespoOfLoggedIn;

            //CHANGE 2 END

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageAdminPage_ExecuteAction(ManageAdminAccountsModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromAdminManagePage_ToEditAdminsPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                //return DeleteSelectedAdmins(model);
                return TransitionFromManagePage_ToDeleteAdminsPage(model);
            }
            else if (executeAction.Equals("CreateAction"))
            {
                return Go_FromManageAdminPage_ToCreateAdminAccountPage();
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageAdminAccountsModel_ThenGoToManagePage(model);
            }
            else if (executeAction.Equals("ConfigurePermissionAction"))
            {
                return TransitionFromAdminManagePage_ToConfigurePermissionPage(model);
            }
            else
            {
                return ManageAdminPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageAdminPage_DoPageIndexChange(ManageAdminAccountsModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }


        //When changing this, change the method in the BaseControllerWithNavBar as well.
        private ActionResult Go_FromManageAdminPage_ToCreateAdminAccountPage()
        {
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManageAdminAccountsModel_ThenGoToManagePage(ManageAdminAccountsModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromAdminManagePage_ToEditAdminsPage(ManageAdminAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAdminAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY, account);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToDeleteAdminsPage(ManageAdminAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAdminAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromAdminManagePage_ToConfigurePermissionPage(ManageAdminAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAdminAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_PERMISSION_OF_ACCOUNT_KEY, account);
                    
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }

        #endregion

        #region "Create/Edit admin account"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO)]
        public ActionResult GoToCreateEditAdminAccountPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditAdminAccountModel)TempData[TEMP_DATA_MODEL_KEY];

            var accountToEdit = (Account)TempData[TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY];

            return View(GetConfiguredCreateEditAdminAccountModel(account, model, accountToEdit));
        }

        private CreateEditAdminAccountModel GetConfiguredCreateEditAdminAccountModel(Account account, CreateEditAdminAccountModel model, Account accountToEdit = null)
        {
            string accountTypeToCreate = TypeConstants.ACCOUNT_TYPE_NORMAL;

            //

            if (model == null)
            {
                model = new CreateEditAdminAccountModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (accountToEdit != null)
            {
                model.AccountId = accountToEdit.Id;
                model.AccountType = accountToEdit.AccountType;
                model.InputUsername = accountToEdit.Username;
                model.InputDisabledFromLogIn = accountToEdit.DisabledFromLogIn;

                model.InputNoChangeOnPassword = true;
            }
            //Supplied account type to create from manage page.
            else if (accountToEdit == null & accountTypeToCreate != null)
            {
                model.AccountType = accountTypeToCreate;
            }

            // DIV

            var adGetParamForCat = new AdvancedGetParameters();
            //var catList = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryAdvancedGetCategoriesAsList(adGetParamForCat);
            var catList = GetDivisionalResponsibilitiesOfAccount(model.LoggedInAccount, adGetParamForCat);

            model.DivisionNameList = new List<string>();

            foreach (Category cat in catList)
            {
                model.DivisionNameList.Add(cat.Name);
            }

            // DIV

            model.InputAccountDivisionNameList = new List<string>();

            var relList = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AccountId, new AdvancedGetParameters());

            if (relList != null)
            {
                foreach (int id in relList)
                {
                    var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);
                    if (cat != null)
                    {
                        model.InputAccountDivisionNameList.Add(cat.Name);

                        if (model.DivisionNameList.Contains(cat.Name))
                        {
                            model.DivisionNameList.Remove(cat.Name);
                        }
                    }
                }
            }

            // DIV RESPO

            var adGetParamForDivRespo = new AdvancedGetParameters();
            //var divRespoList = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryAdvancedGetCategoriesAsList(adGetParamForDivRespo);
            var divRespoList = GetDivisionalResponsibilitiesOfAccount(model.LoggedInAccount, adGetParamForDivRespo);


            model.ToDivisionResponsibilityNameList = new List<string>();

            foreach (Category cat in divRespoList)
            {
                model.ToDivisionResponsibilityNameList.Add(cat.Name);
            }


            // DIV RESPO

            model.InputAccountToDivisionResponibilityNameList = new List<string>();

            var relListOfAccToDiv = accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AccountId, new AdvancedGetParameters());

            if (relListOfAccToDiv != null)
            {
                foreach (int id in relListOfAccToDiv)
                {
                    var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);
                    if (cat != null)
                    {
                        model.InputAccountToDivisionResponibilityNameList.Add(cat.Name);

                        if (model.ToDivisionResponsibilityNameList.Contains(cat.Name))
                        {
                            model.ToDivisionResponsibilityNameList.Remove(cat.Name);
                        }
                    }
                }
            }

            //

            return model;
        }

        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__EXECUTE_ACTION)]
        public ActionResult Execute_CreateEditAdminAccount(CreateEditAdminAccountModel model)
        {
            if (ModelState.IsValid)
            {
                if ((model.ArePasswordAndConfirmEqualAndNotEmptyAndNotNull_OrNoChangeInPassword_OrGeneratePassword() && !model.InputNoChangeOnPassword) || model.InputNoChangeOnPassword)
                {
                    try
                    {
                        if (model.IsActionCreateAccount())
                        {
                            var accIdCreated = AttemptCreateAdminAccount(model);
                            var success = accIdCreated != 1;

                            if (success)
                            {
                                model.StatusMessage = "Account created!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                                model.AccountId = accIdCreated;

                                var accountToEdit = accAccessor.AccountDatabaseManagerHelper.GetAccountInfoFromId(accIdCreated);


                                TempData.Add(TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY, accountToEdit);
                            }
                            else
                            {
                                model.StatusMessage = "Account creation failed!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                            }

                        }
                        else if (model.IsActionEditAccount())
                        {
                            var success = AttemptEditAdminAccount(model);

                            if (success)
                            {
                                model.StatusMessage = "Account edited!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            }
                            else
                            {
                                model.StatusMessage = "Error in editing. Please try again.";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                            }
                        }
                        else
                        {
                            model.StatusMessage = "Error: should not reach here!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }
                    }
                    catch (SqlException e)
                    {
                        model.StatusMessage = string.Format("An error has occured. Please try again. {0}", e.StackTrace);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (InputStringConstraintsViolatedException e)
                    {
                        model.StatusMessage = string.Format("An invalid character has been detected: {0}", e.ViolatingString);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (AccountAlreadyExistsException e)
                    {
                        model.StatusMessage = string.Format("Account with the username already exists: {0}", e.ExistingAccountUsername);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (Exception)
                    {
                        model.StatusMessage = "A generic error has occured. Please try again.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                        
                }
                else
                {
                    model.StatusMessage = "Passwords do not match.";
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ADMIN_ACCOUNT_PAGE__GO_TO, "Admin");
        }

        private int AttemptCreateAdminAccount(CreateEditAdminAccountModel model)
        {
            var builder = new Account.Builder();
            builder.Username = model.InputUsername;
            builder.DisabledFromLogIn = model.InputDisabledFromLogIn;
            builder.AccountType = model.AccountType;

            string password = model.InputPassword;

            if (model.InputGenerateRandomPasswordInstead)
            {
                password = DefaultValueConstants.DEFAULT_PASSWORD;
            }

            var accId = accAccessor.AccountDatabaseManagerHelper.CreateAccount(builder, password);

            //

            var boolBuilder = new BooleanCorrelation.Builder();
            boolBuilder.OwnerId = accId;
            boolBuilder.BoolValue = true;

            accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.CreateBooleanCorr(boolBuilder);

            //

            if (model.InputAccountDivisionNameList != null)
            {
                foreach (string catName in model.InputAccountDivisionNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(accId, accCat.Id);
                    }
                }
            }

            //

            if (model.InputAccountToDivisionResponibilityNameList != null)
            {
                foreach (string catName in model.InputAccountToDivisionResponibilityNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(accId, accCat.Id);
                    }
                }
            }

            //

            return accId;
        }

        private bool AttemptEditAdminAccount(CreateEditAdminAccountModel model)
        {
            var builder = new Account.Builder();
            builder.Username = model.InputUsername;
            builder.DisabledFromLogIn = model.InputDisabledFromLogIn;
            builder.AccountType = model.AccountType;

            string password = null;
            if (!model.InputNoChangeOnPassword)
            {
                password = model.InputPassword;

                if (model.InputGenerateRandomPasswordInstead)
                {
                    password = DefaultValueConstants.DEFAULT_PASSWORD;
                }
            }

            var success = accAccessor.AccountDatabaseManagerHelper.EditAccountWithId(model.AccountId, builder, password);

            //

            accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryDeletePrimaryWithId(model.AccountId);

            if (model.InputAccountDivisionNameList != null)
            {
                foreach (string catName in model.InputAccountDivisionNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AccountId, accCat.Id);
                    }
                }
            }

            //

            accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.TryDeletePrimaryWithId(model.AccountId);

            if (model.InputAccountToDivisionResponibilityNameList != null)
            {
                foreach (string catName in model.InputAccountToDivisionResponibilityNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivRespoAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AccountId, accCat.Id);
                    }
                }
            }

            //

            var boolBuilder = new BooleanCorrelation.Builder();
            boolBuilder.OwnerId = model.AccountId;
            boolBuilder.BoolValue = true;

            if (!accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.IfBooleanCorrelationExsists(model.AccountId)) {
                accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.CreateBooleanCorr(boolBuilder);
            }

            //


            return success;
        }


        #endregion

        #region "ConfirmDelete Admin Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__GO_TO)]
        public ActionResult GoToConfirmDeleteAdminAccounts()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_DELETE_AD_GET_PARAM_KEY];

            var confirmDeleteModel = (ConfirmDeleteAdminAccountModel)TempData[TEMP_DATA_CONFIRM_DELETE_MODEL_KEY];

            return View(GetConfiguredConfigureDeleteAdminAccountsModel(account, adGetParam, confirmDeleteModel));
        }


        private ConfirmDeleteAdminAccountModel GetConfiguredConfigureDeleteAdminAccountsModel(Account account, AdvancedGetParameters adGetParam = null, ConfirmDeleteAdminAccountModel confirmDeleteModel = null)
        {
            if (confirmDeleteModel == null)
            {
                confirmDeleteModel = new ConfirmDeleteAdminAccountModel(account);
            }
            confirmDeleteModel.LoggedInAccount = account;

            //

            var selectedAdminIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS];
            if (selectedAdminIds == null)
            {
                selectedAdminIds = new List<int>();
            }

            /*
            if (confirmDeleteModel.EntityRepresentationsInPage != null)
            {
                foreach (StudentAccountRepresentation accRep in confirmDeleteModel.EntityRepresentationsInPage)
                {
                    if (accRep.IsSelected)
                    {
                        if (!selectedStudIds.Contains(accRep.Id))
                        {
                            selectedStudIds.Add(accRep.Id);
                        }
                    }
                    else
                    {
                        if (selectedStudIds.Contains(accRep.Id))
                        {
                            selectedStudIds.Remove(accRep.Id);
                        }
                    }

                }
            }

            */
            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ADMIN_ACCOUNTS] = selectedAdminIds;
            confirmDeleteModel.SelectedEntityIds = selectedAdminIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAdminAccountsModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var studAccs = new List<Account>();

            for (var i = 0; selectedAdminIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var acc = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAdminIds[i]);
                    if (acc != null)
                    {
                        studAccs.Add(acc);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            confirmDeleteModel.EntitiesInPage = studAccs;

            if (confirmDeleteModel.EntityRepresentationsInPage == null)
            {
                confirmDeleteModel.EntityRepresentationsInPage = new List<AdminAccountRepresentation>();
            }
            else
            {
                confirmDeleteModel.EntityRepresentationsInPage.Clear();
            }


            foreach (Account acc in studAccs)
            {
                var accRep = new AdminAccountRepresentation();
                accRep.Id = acc.Id;
                accRep.Username = acc.Username;
                accRep.DisabledFromLogIn = acc.DisabledFromLogIn;
                accRep.IsSelected = confirmDeleteModel.SelectedEntityIds.Contains(acc.Id);

                confirmDeleteModel.EntityRepresentationsInPage.Add(accRep);
            }


            //

            confirmDeleteModel.TotalEntityCount = selectedAdminIds.Count;

            if (!confirmDeleteModel.IsPageIndexValid(confirmDeleteModel.CurrentPageIndex))
            {
                confirmDeleteModel.CurrentPageIndex = 1;
            }

            return confirmDeleteModel;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__EXECUTE_ACTION)]
        public ActionResult ConfirmDeletePage_AdminExecuteAction(ConfirmDeleteAdminAccountModel model, string executeAction)
        {
            if (executeAction.Equals("ConfirmDelete"))
            {
                return ConfirmDeleteAdminsPage_DeleteSelectedAdmins(model);
            }
            else if (executeAction.Equals("CancelDelete"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
            }
            else
            {
                return ConfirmDeleteAdminPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ConfirmDeleteAdminsPage_DeleteSelectedAdmins(ConfirmDeleteAdminAccountModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredConfigureDeleteAdminAccountsModel(loggedInAccount, null, model);



            var idsToRemove = new List<int>();

            foreach (int id in model.SelectedEntityIds)
            {
                var success = accAccessor.AccountDatabaseManagerHelper.TryDeleteAccountWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            //TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
        }


        private ActionResult ConfirmDeleteAdminPage_DoPageIndexChange(ConfirmDeleteAdminAccountModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = ManageAdminAccountsModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_DELETE_AD_GET_PARAM_KEY, adGetParam);

            //return GoToManageStudentsPage(model, adGetParam);
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ADMIN_ACCOUNT_PAGE__GO_TO, "Admin");
        }


        #endregion




        //

        //UNUSED
        #region "Manage Employee Acc Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO)]
        public ActionResult GoToManageEmployeePage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageEmployeeAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, GetConfiguredManageEmployeeAccountsModel(account, adGetParam, model));
        }

        private ManageEmployeeAccountsModel GetConfiguredManageEmployeeAccountsModel(Account account, AdvancedGetParameters adGetParam = null, ManageEmployeeAccountsModel model = null)
        {
            if (model == null)
            {
                model = new ManageEmployeeAccountsModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedAdminIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];
            if (selectedAdminIds == null)
            {
                selectedAdminIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (EmployeeAccountRepresentation accRep in model.EntityRepresentationsInPage)
                {
                    if (accRep.IsSelected)
                    {
                        if (!selectedAdminIds.Contains(accRep.Id))
                        {
                            selectedAdminIds.Add(accRep.Id);
                        }
                    }
                    else
                    {
                        if (selectedAdminIds.Contains(accRep.Id))
                        {
                            selectedAdminIds.Remove(accRep.Id);
                        }
                    }

                }
            }

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS] = selectedAdminIds;
            model.SelectedEntityIds = selectedAdminIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAdminAccountsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageAdminAccountsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.AccountUsernameFilter;

            model.AdGetParam = adGetParam;

            //

            var employeeAccs = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountsAsList(adGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);
            model.EntitiesInPage = employeeAccs;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<EmployeeAccountRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Account acc in employeeAccs)
            {
                var accRep = new EmployeeAccountRepresentation();
                accRep.Id = acc.Id;
                accRep.Username = acc.Username;
                accRep.DisabledFromLogIn = acc.DisabledFromLogIn;
                accRep.IsSelected = model.SelectedEntityIds.Contains(acc.Id);

                model.EntityRepresentationsInPage.Add(accRep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            model.TotalEntityCount = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountCount(countAdGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageEmployeePage_ExecuteAction(ManageEmployeeAccountsModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromEmployeeManagePage_ToEditEmployeesPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                //return DeleteSelectedAdmins(model);
                return TransitionFromManagePage_ToDeleteEmployeesPage(model);
            }
            else if (executeAction.Equals("CreateAction"))
            {
                return Go_FromManageEmployeePage_ToCreateEmployeeAccountPage();
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageEmployeeAccountsModel_ThenGoToManagePage(model);
            }
            else if (executeAction.Equals("ViewRecordOfSelectedAccount"))
            {
                return GoToViewEmployeeRecord(model);
            }
            else
            {
                return ManageEmployeePage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageEmployeePage_DoPageIndexChange(ManageEmployeeAccountsModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }


        //When changing this, change the method in the BaseControllerWithNavBar as well.
        private ActionResult Go_FromManageEmployeePage_ToCreateEmployeeAccountPage()
        {
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManageEmployeeAccountsModel_ThenGoToManagePage(ManageEmployeeAccountsModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromEmployeeManagePage_ToEditEmployeesPage(ManageEmployeeAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageEmployeeAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY, account);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToDeleteEmployeesPage(ManageEmployeeAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageEmployeeAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }

        private ActionResult GoToViewEmployeeRecord(ManageEmployeeAccountsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageEmployeeAccountsModel(loggedInAccount, null, model);
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];

            if (selectedAccIds != null && selectedAccIds.Count > 0)
            {
                Account account = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedAccIds[0]);

                if (account != null)
                {
                    TempData.Add(GenericUserController.TEMP_DATA_EMPLOYEE_ID_KEY, account.Id);

                    return RedirectToAction(ActionNameConstants.GENERIC_SIDE__VIEW_EDIT_EMPLOYEE_RECORD__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
                }
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }


        #endregion

        //UNUSED
        #region "Create/Edit employee account"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO)]
        public ActionResult GoToCreateEditEmployeeAccountPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditEmployeeAccountModel)TempData[TEMP_DATA_MODEL_KEY];

            var accountToEdit = (Account)TempData[TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY];
            
            return View(GetConfiguredCreateEditEmployeeAccountModel(account, model, accountToEdit));
        }

        private CreateEditEmployeeAccountModel GetConfiguredCreateEditEmployeeAccountModel(Account account, CreateEditEmployeeAccountModel model, Account accountToEdit = null)
        {
            string accountTypeToCreate = TypeConstants.ACCOUNT_TYPE_NORMAL;
            
            //

            if (model == null)
            {
                model = new CreateEditEmployeeAccountModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (accountToEdit != null)
            {
                model.AccountId = accountToEdit.Id;
                model.AccountType = accountToEdit.AccountType;
                model.InputUsername = accountToEdit.Username;
                model.InputDisabledFromLogIn = accountToEdit.DisabledFromLogIn;

                model.InputNoChangeOnPassword = true;
            }
            //Supplied account type to create from manage page.
            else if (accountToEdit == null & accountTypeToCreate != null)
            {
                model.AccountType = accountTypeToCreate;
            }

            //

            model.ListOfEmployeeCategories = GetConfiguredListOfEmployeeCategoriesForChoices(model.ListOfEmployeeCategories);

            //

            /*
            var relList = (HashSet<int>) accToEmployeeDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AccountId, new AdvancedGetParameters());
            if (relList != null)
            {
                var allCats = employeeDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());
                bool isAllCats = relList.Count == allCats.Count;

                if (!isAllCats)
                {
                    foreach (int id in relList)
                    {
                        var cat = employeeDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);
                        if (cat != null)
                        {
                            model.InputChosenEmployeeCategory = cat.Name;
                            break;
                        }
                    }
                }
                else
                {
                    model.InputChosenEmployeeCategory = ANNOUNCE_TO_ALL_ITEM;
                }
            }
            */

            //

            ISet<int> relList = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AccountId, new AdvancedGetParameters());
            if (relList != null)
            {
                var allCats = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());
                bool isAllCats = relList.Count == allCats.Count;

                if (!isAllCats)
                {
                    foreach (int id in relList)
                    {
                        var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);

                        if (cat != null)
                        {
                            model.InputChosenEmployeeCategory = cat.Name;
                            break;
                        }
                    }
                }
                else
                {
                    model.InputChosenEmployeeCategory = ANNOUNCE_TO_ALL_ITEM;
                }
            }
            else
            {
                model.InputChosenEmployeeCategory = CreateEditAdminAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME;
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

            var listOfCats = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());

            list.Add(CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME);
            foreach (Category cat in listOfCats)
            {
                list.Add(cat.Name);
            }

            return list;
        } 


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__EXECUTE_ACTION)]
        public ActionResult Execute_CreateEditEmployeeAccount(CreateEditEmployeeAccountModel model)
        {
            if (ModelState.IsValid)
            {
                if ((model.ArePasswordAndConfirmEqualAndNotEmptyAndNotNull_OrNoChangeInPassword_OrGeneratePassword() && !model.InputNoChangeOnPassword) || model.InputNoChangeOnPassword)
                {
                    try
                    {
                        if (model.IsActionCreateAccount())
                        {
                            var accIdCreated = AttemptCreateEmployeeAccount(model);
                            var success = accIdCreated != 1;

                            if (success)
                            {
                                model.StatusMessage = "Account created!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                                model.AccountId = accIdCreated;

                                var accountToEdit = accAccessor.AccountDatabaseManagerHelper.GetAccountInfoFromId(accIdCreated);


                                TempData.Add(TEMP_DATA_EDIT_GENERIC_ACCOUNT_KEY, accountToEdit);
                            }
                            else
                            {
                                model.StatusMessage = "Account creation failed!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                            }

                        }
                        else if (model.IsActionEditAccount())
                        {
                            var success = AttemptEditEmployeeAccount(model);

                            if (success)
                            {
                                model.StatusMessage = "Account edited!";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            }
                            else
                            {
                                model.StatusMessage = "Error in editing. Please try again.";
                                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                            }
                        }
                        else
                        {
                            model.StatusMessage = "Error: should not reach here!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }
                    }
                    catch (SqlException)
                    {
                        model.StatusMessage = "An error has occured. Please try again.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (InputStringConstraintsViolatedException e)
                    {
                        model.StatusMessage = string.Format("An invalid character has been detected: {0}", e.ViolatingString);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (AccountAlreadyExistsException e)
                    {
                        model.StatusMessage = string.Format("Account with the username already exists: {0}", e.ExistingAccountUsername);
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                    catch (Exception)
                    {
                        model.StatusMessage = "A generic error has occured. Please try again.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                else
                {
                    model.StatusMessage = "Passwords do not match.";
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_EMPLOYEE_ACCOUNT_PAGE__GO_TO, "Admin");
        }

        private int AttemptCreateEmployeeAccount(CreateEditEmployeeAccountModel model)
        {
            var builder = new Account.Builder();
            builder.Username = model.InputUsername;
            builder.DisabledFromLogIn = model.InputDisabledFromLogIn;
            builder.AccountType = model.AccountType;

            string password = model.InputPassword;

            if (model.InputGenerateRandomPasswordInstead)
            {
                password = DefaultValueConstants.DEFAULT_PASSWORD;
            }

            var accId = accAccessor.AccountDatabaseManagerHelper.CreateAccount(builder, password);

            //

            var boolBuilder = new BooleanCorrelation.Builder();
            boolBuilder.OwnerId = accId;
            boolBuilder.BoolValue = true;

            accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.CreateBooleanCorr(boolBuilder);

            //

            if (accId != -1 && !CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME.Equals(model.InputChosenEmployeeCategory))
            {
                var category = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(model.InputChosenEmployeeCategory);

                if (category != null)
                {
                    if (!accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.IfRelationExsists(model.AccountId, category.Id))
                    {
                        var relationCreated = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryCreatePrimaryToTargetRelation(accId, category.Id);
                    }
                }
            }

            //

            return accId;
        }

        private bool AttemptEditEmployeeAccount(CreateEditEmployeeAccountModel model)
        {
            var builder = new Account.Builder();
            builder.Username = model.InputUsername;
            builder.DisabledFromLogIn = model.InputDisabledFromLogIn;
            builder.AccountType = model.AccountType;

            string password = null;
            if (!model.InputNoChangeOnPassword)
            {
                password = model.InputPassword;

                if (model.InputGenerateRandomPasswordInstead)
                {
                    password = DefaultValueConstants.DEFAULT_PASSWORD;
                }
            }

            var success = accAccessor.AccountDatabaseManagerHelper.EditAccountWithId(model.AccountId, builder, password);

            //

            if (model.AccountId != -1 && !CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME.Equals(model.InputChosenEmployeeCategory))
            {
                var category = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(model.InputChosenEmployeeCategory);

                if (category != null)
                {
                    if (accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.IfPrimaryExsists(model.AccountId))
                    {
                        accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(model.AccountId);
                        
                    }

                    var relationCreated = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AccountId, category.Id);
                }
            }
            else if (CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME.Equals(model.InputChosenEmployeeCategory))
            {
                if (accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.IfPrimaryExsists(model.AccountId))
                {
                    accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(model.AccountId);
                }
            }

            //

            return success;
        }


        #endregion

        //UNUSED
        #region "ConfirmDelete Employee Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__GO_TO)]
        public ActionResult GoToConfirmDeleteEmployeeAccounts()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_DELETE_AD_GET_PARAM_KEY];

            var confirmDeleteModel = (ConfirmDeleteEmployeeAccountModel)TempData[TEMP_DATA_CONFIRM_DELETE_MODEL_KEY];

            return View(GetConfiguredConfigureDeleteEmployeeAccountsModel(account, adGetParam, confirmDeleteModel));
        }


        private ConfirmDeleteEmployeeAccountModel GetConfiguredConfigureDeleteEmployeeAccountsModel(Account account, AdvancedGetParameters adGetParam = null, ConfirmDeleteEmployeeAccountModel confirmDeleteModel = null)
        {
            if (confirmDeleteModel == null)
            {
                confirmDeleteModel = new ConfirmDeleteEmployeeAccountModel(account);
            }
            confirmDeleteModel.LoggedInAccount = account;

            //

            var selectedEmployeeIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS];
            if (selectedEmployeeIds == null)
            {
                selectedEmployeeIds = new List<int>();
            }

            /*
            if (confirmDeleteModel.EntityRepresentationsInPage != null)
            {
                foreach (StudentAccountRepresentation accRep in confirmDeleteModel.EntityRepresentationsInPage)
                {
                    if (accRep.IsSelected)
                    {
                        if (!selectedStudIds.Contains(accRep.Id))
                        {
                            selectedStudIds.Add(accRep.Id);
                        }
                    }
                    else
                    {
                        if (selectedStudIds.Contains(accRep.Id))
                        {
                            selectedStudIds.Remove(accRep.Id);
                        }
                    }

                }
            }

            */
            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_EMPLOYEE_ACCOUNTS] = selectedEmployeeIds;
            confirmDeleteModel.SelectedEntityIds = selectedEmployeeIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageEmployeeAccountsModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var studAccs = new List<Account>();

            for (var i = 0; selectedEmployeeIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var acc = accAccessor.AccountDatabaseManagerHelper.TryGetAccountInfoFromId(selectedEmployeeIds[i]);
                    if (acc != null)
                    {
                        studAccs.Add(acc);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            confirmDeleteModel.EntitiesInPage = studAccs;

            if (confirmDeleteModel.EntityRepresentationsInPage == null)
            {
                confirmDeleteModel.EntityRepresentationsInPage = new List<EmployeeAccountRepresentation>();
            }
            else
            {
                confirmDeleteModel.EntityRepresentationsInPage.Clear();
            }


            foreach (Account acc in studAccs)
            {
                var accRep = new EmployeeAccountRepresentation();
                accRep.Id = acc.Id;
                accRep.Username = acc.Username;
                accRep.DisabledFromLogIn = acc.DisabledFromLogIn;
                accRep.IsSelected = confirmDeleteModel.SelectedEntityIds.Contains(acc.Id);

                confirmDeleteModel.EntityRepresentationsInPage.Add(accRep);
            }


            //

            confirmDeleteModel.TotalEntityCount = selectedEmployeeIds.Count;

            if (!confirmDeleteModel.IsPageIndexValid(confirmDeleteModel.CurrentPageIndex))
            {
                confirmDeleteModel.CurrentPageIndex = 1;
            }

            return confirmDeleteModel;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__EXECUTE_ACTION)]
        public ActionResult ConfirmDeletePage_EmployeeExecuteAction(ConfirmDeleteEmployeeAccountModel model, string executeAction)
        {
            if (executeAction.Equals("ConfirmDelete"))
            {
                return ConfirmDeleteEmployeesPage_DeleteSelectedEmployees(model);
            }
            else if (executeAction.Equals("CancelDelete"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
            }
            else
            {
                return ConfirmDeleteEmployeePage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ConfirmDeleteEmployeesPage_DeleteSelectedEmployees(ConfirmDeleteEmployeeAccountModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredConfigureDeleteEmployeeAccountsModel(loggedInAccount, null, model);


            var idsToRemove = new List<int>();

            foreach (int id in model.SelectedEntityIds)
            {
                var targetIds = empAccToEmpRecordAccessor.EntityToRecordDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(id, new AdvancedGetParameters());
                foreach (int targetId in targetIds)
                {
                    var delSuccess = employeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryDeleteEmployeeRecordWithId(targetId);
                    break;
                }

                //

                var success = accAccessor.AccountDatabaseManagerHelper.TryDeleteAccountWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            //TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_EMPLOYEE_PAGE__GO_TO, "Admin");
        }


        private ActionResult ConfirmDeleteEmployeePage_DoPageIndexChange(ConfirmDeleteEmployeeAccountModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = ManageEmployeeAccountsModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_DELETE_AD_GET_PARAM_KEY, adGetParam);

            //return GoToManageStudentsPage(model, adGetParam);
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_EMPLOYEE_ACCOUNT_PAGE__GO_TO, "Admin");
        }


        #endregion


        //

        #region "Manage Announcements Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO)]
        public ActionResult GoToManageAnnouncementsPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageAnnouncementsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, GetConfiguredManageAnnouncementsModel(account, adGetParam, model));
        }

        private ManageAnnouncementsModel GetConfiguredManageAnnouncementsModel(Account account, AdvancedGetParameters adGetParam = null, ManageAnnouncementsModel model = null)
        {
            if (model == null)
            {
                model = new ManageAnnouncementsModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (AnnouncementRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAnnouncementsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageAnnouncementsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            //var entities = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);
            currentAdGetParam = adGetParam;
            currentLoggedInAccount = model.LoggedInAccount;

            var annIdsInDivRespoOfLoggedIn = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedRelationsSatisfyingCondition(adGetParam,
                IsAnnouncementInAllDivisionResponsibilityOfLoggedIn,
                false
                );
            var entities = new List<Announcement>();
            foreach (Relation rel in annIdsInDivRespoOfLoggedIn)
            {
                entities.Add(annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(rel.PrimaryId));
            }



            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<AnnouncementRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Announcement ent in entities)
            {
                var rep = new AnnouncementRepresentation();
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
            currentAdGetParam = countAdGetParam;
            var allAnnIdCountInDivRespoOfLoggedIn = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationCountSatisfyingCondition(countAdGetParam,
                IsAnnouncementInAllDivisionResponsibilityOfLoggedIn,
                false
                );
            model.TotalEntityCount = allAnnIdCountInDivRespoOfLoggedIn;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageAnnouncementPage_ExecuteAction(ManageAnnouncementsModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromAnnouncementManagePage_ToEditAnnouncementsPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                //return DeleteSelectedAdmins(model);
                return TransitionFromManagePage_ToDeleteAnnouncementPage(model);
            }
            else if (executeAction.Equals("CreateAction"))
            {
                return Go_FromManageAnnouncementPage_ToCreateAnnouncementPage();
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageAnnouncementModel_ThenGoToManagePage(model);
            }
            else
            {
                return ManageAnnouncementPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageAnnouncementPage_DoPageIndexChange(ManageAnnouncementsModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }


        //When changing this, change the method in the BaseControllerWithNavBar as well.
        private ActionResult Go_FromManageAnnouncementPage_ToCreateAnnouncementPage()
        {
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManageAnnouncementModel_ThenGoToManagePage(ManageAnnouncementsModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromAnnouncementManagePage_ToEditAnnouncementsPage(ManageAnnouncementsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAnnouncementsModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Announcement announcement = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(selectedIds[0]);

                if (announcement != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_ANNOUNCEMENT_KEY, announcement);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToDeleteAnnouncementPage(ManageAnnouncementsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAnnouncementsModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Announcement announcement = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(selectedIds[0]);

                if (announcement != null)
                {
                    TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }

        #endregion


        #region "Create/Edit Announcement"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__GO_TO)]
        public ActionResult GoToCreateEditAnnouncementPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditAnnouncementModel)TempData[TEMP_DATA_MODEL_KEY];

            var announcementToEdit = (Announcement)TempData[TEMP_DATA_EDIT_ANNOUNCEMENT_KEY];

            /*
            var refreshImage = true;
            if (TempData.Keys.Contains(TEMP_DATA_ANNOUNCEMENT_REFRESH_IMAGE_KEY))
            {
                refreshImage = (bool)TempData[TEMP_DATA_ANNOUNCEMENT_REFRESH_IMAGE_KEY];
            }
            */

            return View(GetConfiguredCreateEditAnnouncementModel(account, model, announcementToEdit));
        }


        private CreateEditAnnouncementModel GetConfiguredCreateEditAnnouncementModel(Account account, CreateEditAnnouncementModel model, Announcement announcementToEdit = null)
        {
            if (model == null)
            {
                model = new CreateEditAnnouncementModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (announcementToEdit != null)
            {
                model.AnnouncementId = announcementToEdit.Id;
                model.InputTitle = announcementToEdit.Title;
                model.InputContent = announcementToEdit.GetContentAsString();

                var imageContent = imageAccessor.ImageDatabaseManagerHelper.TryGetImageContentInfoFromId(announcementToEdit.MainImageId);

                if (imageContent != null)
                {
                    var absPath = imageContent.ImageFullPath;
                    model.AnnouncementImagePath = UrlResolver.RelativePath(absPath, System.Web.HttpContext.Current.Request);
                }

                //model.InputImage = announcementToEdit.MainImage;

            }

            //Supplied account type to create from manage page.
            else if (announcementToEdit == null)
            {

            }

            //


            //var adGetParamForCat = GetAdGetParam_ForCategoriesOfAnnouncements_InCreateEditAnnouncement();


            var adGetParamForCat = new AdvancedGetParameters();
            //var catList = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryAdvancedGetCategoriesAsList(adGetParamForCat);
            var catList = GetDivisionalResponsibilitiesOfAccount(model.LoggedInAccount, adGetParamForCat);

            model.DivisionNameList = new List<string>();

            foreach (Category cat in catList)
            {
                model.DivisionNameList.Add(cat.Name);
            }

            // DIV

            model.InputAccountDivisionNameList = new List<string>();

            var relList = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AnnouncementId, new AdvancedGetParameters());

            if (relList != null)
            {
                foreach (int id in relList)
                {
                    var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);
                    if (cat != null)
                    {
                        model.InputAccountDivisionNameList.Add(cat.Name);

                        if (model.DivisionNameList.Contains(cat.Name))
                        {
                            model.DivisionNameList.Remove(cat.Name);
                        }
                    }
                }
            }

            //


            return model;
        }

        /*
        [ActionName(ActionNameConstants.ADMIN_SIDE_CONVERT_IMAGE_BYTES_TO_URL_ACTION)]
        public ActionResult RenderImageFromBytes(string imgBytes)
        {
            return File(StringUtilities.ConvertStringToByteArray(imgBytes), "image/png");
        }
        */


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__EXECUTE_ACTION)]
        public ActionResult ExecuteAction_CreateEditAnnouncement(CreateEditAnnouncementModel model, string executeAction)
        {
            if (executeAction != null && executeAction.Equals("SaveChanges"))
            {
                return ActionSaveAnnouncementAndImage(model);
            }
            else
            {
                return Content(executeAction); //error
            }
        }

        /*
        private ActionResult ActionUpdateAnnouncmentImage(CreateEditAnnouncementModel model)
        {
            HttpPostedFileBase file = Request.Files["ImageData"];

            TempData.Add(TEMP_DATA_MODEL_KEY, model);


            return RedirectToAction(ActionNameConstants.ADMIN_SIDE_GO_TO_CREATE_EDIT_ANNOUNCEMENT_PAGE, "Admin");
        }

        private byte[] ConvertPostedFileToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        */


        private ActionResult ActionSaveAnnouncementAndImage(CreateEditAnnouncementModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsActionCreateAnnouncement())
                    {
                        var annIdCreated = AttemptCreateAnnouncementAndImage(model);
                        var success = annIdCreated != -1;

                        if (success)
                        {
                            model.StatusMessage = "Announcement created!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            model.AnnouncementId = annIdCreated;

                            var announcementToEdit = annAccessor.AnnouncementManagerHelper.GetAnnouncementInfoFromId(annIdCreated);

                            TempData.Add(TEMP_DATA_EDIT_ANNOUNCEMENT_KEY, announcementToEdit);
                        }
                        else
                        {
                            model.StatusMessage = "Announcement creation failed!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                    }
                    else if (model.IsActionEditAnnouncement())
                    {
                        var success = AttemptEditAnnouncement(model);

                        if (success)
                        {
                            model.StatusMessage = "Announcmenet edited!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;

                        }
                        else
                        {
                            model.StatusMessage = "Error in editing. Please try again.";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                        TempData.Add(TEMP_DATA_EDIT_ANNOUNCEMENT_KEY, annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(model.AnnouncementId));
                    }
                    else
                    {
                        model.StatusMessage = "Error: should not reach here!";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                catch (SqlException e)
                {
                    model.StatusMessage = string.Format("An error has occured. Please try again. {0}.", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (Exception e)
                {
                    model.StatusMessage = string.Format("A generic error has occured. Please try again. {0}", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }

            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }



        private int AttemptCreateAnnouncementAndImage(CreateEditAnnouncementModel model)
        {
            int imageId = -1;

            if (model.InputImage != null)
            {
                var imageBuilder = new ImageContent.Builder();
                imageBuilder.DataType = ImageDataType.GetStringAsType(Path.GetExtension(model.InputImage.FileName));
                imageBuilder.StoreType = ImageStoreType.TYPE_DIRECTORY_PATH;
                imageBuilder.ImageDirectoryPath = DefaultValueConstants.IMAGE_MANAGER_FOLDER_PATH;

                var fileBytes = new byte[model.InputImage.ContentLength];
                model.InputImage.InputStream.Read(fileBytes, 0, fileBytes.Length);

                imageBuilder.ImageBytes = fileBytes;

                imageId = imageAccessor.ImageDatabaseManagerHelper.CreateImageContent(imageBuilder);
            }

            //

            var builder = new Announcement.Builder();
            builder.Title = model.InputTitle;
            builder.Content = StringUtilities.ConvertStringToByteArray(model.InputContent);

            if (model.InputImage != null)
            {
                builder.MainImageId = imageId;
            }

            var annId = annAccessor.AnnouncementManagerHelper.CreateAnnouncement(builder);

            //

            if (model.InputAccountDivisionNameList != null)
            {
                foreach (string catName in model.InputAccountDivisionNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(annId, accCat.Id);
                    }
                }
            }

            //

            return annId;
        }

        private bool AttemptEditAnnouncement(CreateEditAnnouncementModel model)
        {
            int imageId = -1;

            if (model.InputImage != null)
            {
                var imageBuilder = new ImageContent.Builder();
                imageBuilder.DataType = ImageDataType.GetStringAsType(Path.GetExtension(model.InputImage.FileName));
                imageBuilder.StoreType = ImageStoreType.TYPE_DIRECTORY_PATH;
                imageBuilder.ImageDirectoryPath = DefaultValueConstants.IMAGE_MANAGER_FOLDER_PATH;

                var fileBytes = new byte[model.InputImage.ContentLength];
                model.InputImage.InputStream.Read(fileBytes, 0, fileBytes.Length);

                imageBuilder.ImageBytes = fileBytes;

                imageId = imageAccessor.ImageDatabaseManagerHelper.CreateImageContent(imageBuilder);
            }

            //

            var builder = new Announcement.Builder();
            builder.Title = model.InputTitle;
            builder.Content = StringUtilities.ConvertStringToByteArray(model.InputContent);

            if (model.InputImage != null)
            {
                builder.MainImageId = imageId;
            }

            //

            annToCatAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(model.AnnouncementId);
            if (model.InputAccountDivisionNameList != null)
            {
                foreach (string catName in model.InputAccountDivisionNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AnnouncementId, accCat.Id);
                    }
                }
            }


            return annAccessor.AnnouncementManagerHelper.EditAnnouncement(model.AnnouncementId, builder);
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__ANNOUNCEMENT_CATEGORY__QUERY_ACTION)]
        public JsonResult GetCategoriesOfAnnouncements_BasedOnCurrentValue(string searchValue)
        {
            var adGetParam = GetAdGetParam_ForCategoriesOfAnnouncements_InCreateEditAnnouncement();
            adGetParam.TextToContain = searchValue;

            var catList = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryAdvancedGetCategoriesAsList(adGetParam);

            //throw new Exception(string.Format("Thrown exception {0}, {1}", searchValue, catList[0]));

            return Json(catList, JsonRequestBehavior.AllowGet);
        }

        private AdvancedGetParameters GetAdGetParam_ForCategoriesOfAnnouncements_InCreateEditAnnouncement()
        {
            var adGetParam = new AdvancedGetParameters();
            adGetParam.Fetch = 6;

            return adGetParam;
        }


        #endregion


        #region "ConfirmDelete Announcement Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__GO_TO)]
        public ActionResult GoToConfirmDeleteAnnouncement()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_DELETE_AD_GET_PARAM_KEY];

            var confirmDeleteModel = (ConfirmDeleteAnnouncementModel)TempData[TEMP_DATA_CONFIRM_DELETE_MODEL_KEY];

            return View(GetConfiguredConfigureDeleteAnnouncementModel(account, adGetParam, confirmDeleteModel));
        }


        private ConfirmDeleteAnnouncementModel GetConfiguredConfigureDeleteAnnouncementModel(Account account, AdvancedGetParameters adGetParam = null, ConfirmDeleteAnnouncementModel confirmDeleteModel = null)
        {
            if (confirmDeleteModel == null)
            {
                confirmDeleteModel = new ConfirmDeleteAnnouncementModel();
            }
            confirmDeleteModel.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }


            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ANNOUNCEMENTS] = selectedIds;
            confirmDeleteModel.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ConfirmDeleteAnnouncementModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var entities = new List<Announcement>();

            for (var i = 0; selectedIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var entity = annAccessor.AnnouncementManagerHelper.TryGetAnnouncementInfoFromId(selectedIds[i]);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            confirmDeleteModel.EntitiesInPage = entities;

            if (confirmDeleteModel.EntityRepresentationsInPage == null)
            {
                confirmDeleteModel.EntityRepresentationsInPage = new List<AnnouncementRepresentation>();
            }
            else
            {
                confirmDeleteModel.EntityRepresentationsInPage.Clear();
            }


            foreach (Announcement ann in entities)
            {
                var entityRep = new AnnouncementRepresentation();
                entityRep.Id = ann.Id;
                entityRep.Title = ann.Title;
                entityRep.ContentPreview = ann.GetContentAsString();
                entityRep.IsSelected = confirmDeleteModel.SelectedEntityIds.Contains(ann.Id);

                confirmDeleteModel.EntityRepresentationsInPage.Add(entityRep);
            }


            //

            confirmDeleteModel.TotalEntityCount = selectedIds.Count;

            if (!confirmDeleteModel.IsPageIndexValid(confirmDeleteModel.CurrentPageIndex))
            {
                confirmDeleteModel.CurrentPageIndex = 1;
            }

            return confirmDeleteModel;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__EXECUTE_ACTION)]
        public ActionResult ConfirmDeletePage_AnnouncementExecuteAction(ConfirmDeleteAnnouncementModel model, string executeAction)
        {
            if (executeAction.Equals("ConfirmDelete"))
            {
                return ConfirmDeleteAnnouncementsPage_DeleteSelectedAnnouncements(model);
            }
            else if (executeAction.Equals("CancelDelete"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
            }
            else
            {
                return ConfirmDeleteAnnouncementsPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ConfirmDeleteAnnouncementsPage_DeleteSelectedAnnouncements(ConfirmDeleteAnnouncementModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredConfigureDeleteAnnouncementModel(loggedInAccount, null, model);


            var idsToRemove = new List<int>();

            foreach (int id in model.SelectedEntityIds)
            {
                var imageIdList = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(id, new AdvancedGetParameters());
                int imageId = -1;
                foreach (int i in imageIdList)
                {
                    imageId = i;
                    break;
                }

                if (imageAccessor.ImageDatabaseManagerHelper.IfImageContentIdExsists(imageId))
                {
                    imageAccessor.ImageDatabaseManagerHelper.DeleteImageContentWithId(imageId);
                }


                //

                var success = annAccessor.AnnouncementManagerHelper.TryDeleteAnnouncementWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            //TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }


        private ActionResult ConfirmDeleteAnnouncementsPage_DoPageIndexChange(ConfirmDeleteAnnouncementModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = ManageAnnouncementsModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_DELETE_AD_GET_PARAM_KEY, adGetParam);

            //return GoToManageStudentsPage(model, adGetParam);
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ANNOUNCEMENT_PAGE__GO_TO, "Admin");
        }





        #endregion


        //


        #region "Manage Permissions Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_PERMISSION_PAGE__GO_TO)]
        public ActionResult GoToManagePermissionPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManagePermissionsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_ANNOUNCEMENT_PAGE__GO_TO, GetConfiguredManagePermissionsModel(account, adGetParam, model));
        }

        private ManagePermissionsModel GetConfiguredManagePermissionsModel(Account account, AdvancedGetParameters adGetParam = null, ManagePermissionsModel model = null)
        {
            if (model == null)
            {
                model = new ManagePermissionsModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_PERMISSIONS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (PermissionRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_PERMISSIONS] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAnnouncementsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageAnnouncementsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            var entities = permissionAccessor.PermissionDatabaseManagerHelper.AdvancedGetPermissionsAsList(adGetParam);
            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<PermissionRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Permission ent in entities)
            {
                var rep = new PermissionRepresentation();
                rep.Id = ent.Id;
                rep.Title = ent.Name;
                rep.ContentPreview = ent.GetDescriptionAsString();
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            //TODO Make advanced get count for permission acc
            model.TotalEntityCount = permissionAccessor.PermissionDatabaseManagerHelper.AdvancedGetPermissionCount(countAdGetParam);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_PERMISSION_PAGE__EXECUTE_ACTION)]
        public ActionResult ManagePermissionPage_ExecuteAction(ManagePermissionsModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromPermissionManagePage_ToEditPermissionPage(model);
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManagePermissionModel_ThenGoToManagePage(model);
            }
            else
            {
                return ManagePermissionPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManagePermissionPage_DoPageIndexChange(ManagePermissionsModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_PERMISSION_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManagePermissionModel_ThenGoToManagePage(ManagePermissionsModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_PERMISSIONS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_PERMISSION_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromPermissionManagePage_ToEditPermissionPage(ManagePermissionsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManagePermissionsModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_PERMISSIONS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Permission permission = permissionAccessor.PermissionDatabaseManagerHelper.TryGetPermissionInfoFromId(selectedIds[0]);

                if (permission != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_PERMISSION_KEY, permission);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_PERMISSION_PAGE__GO_TO, "Admin");
        }



        #endregion

        //UNUSED
        #region "Create/Edit Permissions"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__GO_TO)]
        public ActionResult GoToCreateEditPermissionPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditPermissionModel)TempData[TEMP_DATA_MODEL_KEY];

            var permToEdit = (Permission)TempData[TEMP_DATA_EDIT_PERMISSION_KEY];

            /*
            var refreshImage = true;
            if (TempData.Keys.Contains(TEMP_DATA_ANNOUNCEMENT_REFRESH_IMAGE_KEY))
            {
                refreshImage = (bool)TempData[TEMP_DATA_ANNOUNCEMENT_REFRESH_IMAGE_KEY];
            }
            */

            return View(GetConfiguredCreateEditPermissionModel(account, model, permToEdit));
        }


        private CreateEditPermissionModel GetConfiguredCreateEditPermissionModel(Account account, CreateEditPermissionModel model, Permission permissionToEdit = null)
        {
            if (model == null)
            {
                model = new CreateEditPermissionModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (permissionToEdit != null)
            {
                model.PermissionId = permissionToEdit.Id;
                model.InputName = permissionToEdit.Name;
                model.InputDescription = permissionToEdit.GetDescriptionAsString();
            }

            //Supplied account type to create from manage page.
            else if (permissionToEdit == null)
            {

            }

            //

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__EXECUTE_ACTION)]
        public ActionResult ExecuteAction_CreateEditPermission(CreateEditPermissionModel model, string executeAction)
        {
            if (executeAction != null && executeAction.Equals("SaveChanges"))
            {
                return ActionSavePermission(model);
            }
            else
            {
                return Content(executeAction); //error
            }
        }

        private ActionResult ActionSavePermission(CreateEditPermissionModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsActionCreatePermission())
                    {
                        var permIdCreated = AttemptCreatePermission(model);
                        var success = permIdCreated != -1;

                        if (success)
                        {
                            model.StatusMessage = "Permission created!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            model.PermissionId = permIdCreated;

                            var permToEdit = permissionAccessor.PermissionDatabaseManagerHelper.GetPermissionInfoFromId(permIdCreated);

                            TempData.Add(TEMP_DATA_EDIT_PERMISSION_KEY, permToEdit);
                        }
                        else
                        {
                            model.StatusMessage = "Permission creation failed!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                    }
                    else if (model.IsActionEditPermission())
                    {
                        var success = AttemptEditPermission(model);

                        if (success)
                        {
                            model.StatusMessage = "Permission edited!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;

                        }
                        else
                        {
                            model.StatusMessage = "Error in editing. Please try again.";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                        TempData.Add(TEMP_DATA_EDIT_PERMISSION_KEY, permissionAccessor.PermissionDatabaseManagerHelper.TryGetPermissionInfoFromId(model.PermissionId));
                    }
                    else
                    {
                        model.StatusMessage = "Error: should not reach here!";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                catch (SqlException e)
                {
                    model.StatusMessage = string.Format("An error has occured. Please try again. {0}.", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (Exception e)
                {
                    model.StatusMessage = string.Format("A generic error has occured. Please try again. {0}", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }

            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_PERMISSION_PAGE__GO_TO, "Admin");
        }



        private int AttemptCreatePermission(CreateEditPermissionModel model)
        {
            var builder = new Permission.Builder();
            builder.Name = model.InputName;
            builder.Description = StringUtilities.ConvertStringToByteArray(model.InputDescription);

            var permId = permissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder);

            //

            return permId;
        }

        private bool AttemptEditPermission(CreateEditPermissionModel model)
        {
            var builder = new Permission.Builder();
            builder.Name = model.InputName;
            builder.Description = StringUtilities.ConvertStringToByteArray(model.InputDescription);

            //

            return permissionAccessor.PermissionDatabaseManagerHelper.EditPermission(model.PermissionId, builder);
        }



        #endregion


        //


        #region "Manage Requestable Documents Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO)]
        public ActionResult GoToManageRequestableDocumentPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageRequestableDocumentsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, GetConfiguredManageRequestableDocumentModel(account, adGetParam, model));
        }

        private ManageRequestableDocumentsModel GetConfiguredManageRequestableDocumentModel(Account account, AdvancedGetParameters adGetParam = null, ManageRequestableDocumentsModel model = null)
        {
            if (model == null)
            {
                model = new ManageRequestableDocumentsModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (RequestableDocumentRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageRequestableDocumentsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageRequestableDocumentsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            var entities = requestDocuAccessor.ReqDocuManagerHelper.AdvancedGetRequestableDocumentsAsList(adGetParam);
            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<RequestableDocumentRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (RequestableDocument ent in entities)
            {
                var rep = new RequestableDocumentRepresentation();
                rep.Id = ent.Id;
                rep.Title = ent.DocumentName;
                rep.ContentPreview = ent.GetNoteDescriptionAsString();
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            model.TotalEntityCount = requestDocuAccessor.ReqDocuManagerHelper.AdvancedGetRequestableDocumentCount(countAdGetParam);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageRequestDocumentPage_ExecuteAction(ManageRequestableDocumentsModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromRequestableDocumentManagePage_ToEditReqDocuPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                //return DeleteSelectedAdmins(model);
                return TransitionFromManagePage_ToDeleteRequestableDocumentPage(model);
            }
            else if (executeAction.Equals("CreateAction"))
            {
                return Go_FromManageRequestableDocumentPage_ToCreateReqDocuPage();
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageRequestableDocumentModel_ThenGoToManagePage(model);
            }
            else
            {
                return ManageRequestableDocumentPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageRequestableDocumentPage_DoPageIndexChange(ManageRequestableDocumentsModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }


        //When changing this, change the method in the BaseControllerWithNavBar as well.
        private ActionResult Go_FromManageRequestableDocumentPage_ToCreateReqDocuPage()
        {
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManageRequestableDocumentModel_ThenGoToManagePage(ManageRequestableDocumentsModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromRequestableDocumentManagePage_ToEditReqDocuPage(ManageRequestableDocumentsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageRequestableDocumentModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                RequestableDocument reqDocu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(selectedIds[0]);

                if (reqDocu != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_REQUESTABLE_DOCU_KEY, reqDocu);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToDeleteRequestableDocumentPage(ManageRequestableDocumentsModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageRequestableDocumentModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                RequestableDocument reqDocu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(selectedIds[0]);

                if (reqDocu != null)
                {
                    TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }

        #endregion


        #region "Create/Edit Requestable Documents"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO)]
        public ActionResult GoToCreateEditRequestableDocumentPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditRequestableDocumentModel)TempData[TEMP_DATA_MODEL_KEY];

            var requestableDocuToEdit = (RequestableDocument)TempData[TEMP_DATA_EDIT_REQUESTABLE_DOCU_KEY];

            return View(GetConfiguredCreateEditRequestableDocumentModel(account, model, requestableDocuToEdit));
        }


        private CreateEditRequestableDocumentModel GetConfiguredCreateEditRequestableDocumentModel(Account account, CreateEditRequestableDocumentModel model, RequestableDocument requestableDocuToEdit = null)
        {
            if (model == null)
            {
                model = new CreateEditRequestableDocumentModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (requestableDocuToEdit != null)
            {
                model.RequestableDocumentId = requestableDocuToEdit.Id;
                model.InputName = requestableDocuToEdit.DocumentName;
                model.InputContent = requestableDocuToEdit.GetNoteDescriptionAsString();

            }

            //Supplied account type to create from manage page.
            else if (requestableDocuToEdit == null)
            {

            }

            //

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION)]
        public ActionResult ExecuteAction_CreateEditRequestableDocument(CreateEditRequestableDocumentModel model, string executeAction)
        {
            if (executeAction != null && executeAction.Equals("SaveChanges"))
            {
                return ActionSaveRequestableDocument(model);
            }
            else
            {
                return Content(executeAction); //error
            }
        }

        private ActionResult ActionSaveRequestableDocument(CreateEditRequestableDocumentModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsActionCreateRequestableDocument())
                    {
                        var reqDocuIdCreated = AttemptCreateRequestableDocument(model);
                        var success = reqDocuIdCreated != -1;

                        if (success)
                        {
                            model.StatusMessage = "Requestable document created!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            model.RequestableDocumentId = reqDocuIdCreated;

                            var reqDocuToEdit = requestDocuAccessor.ReqDocuManagerHelper.GetRequestableDocumentFromId(reqDocuIdCreated);

                            TempData.Add(TEMP_DATA_EDIT_REQUESTABLE_DOCU_KEY, reqDocuToEdit);
                        }
                        else
                        {
                            model.StatusMessage = "Requestable document creation failed!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                    }
                    else if (model.IsActionEditRequestableDocument())
                    {
                        var success = AttemptEditRequestableDocument(model);

                        if (success)
                        {
                            model.StatusMessage = "Requestable Document edited!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;

                        }
                        else
                        {
                            model.StatusMessage = "Error in editing. Please try again.";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                        TempData.Add(TEMP_DATA_EDIT_REQUESTABLE_DOCU_KEY, requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(model.RequestableDocumentId));
                    }
                    else
                    {
                        model.StatusMessage = "Error: should not reach here!";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                catch (SqlException e)
                {
                    model.StatusMessage = string.Format("An error has occured. Please try again. {0}.", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (Exception e)
                {
                    model.StatusMessage = string.Format("A generic error has occured. Please try again. {0}", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }

            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }



        private int AttemptCreateRequestableDocument(CreateEditRequestableDocumentModel model)
        {
            var builder = new RequestableDocument.Builder();
            builder.DocumentName = model.InputName;
            builder.SetNoteDescriptionWithString(model.InputContent);

            var reqDocuId = requestDocuAccessor.ReqDocuManagerHelper.CreateRequestableDocument(builder);

            return reqDocuId;
        }

        private bool AttemptEditRequestableDocument(CreateEditRequestableDocumentModel model)
        {

            var builder = new RequestableDocument.Builder();
            builder.DocumentName = model.InputName;
            builder.SetNoteDescriptionWithString(model.InputContent);

            //

            return requestDocuAccessor.ReqDocuManagerHelper.EditRequestableDocument(model.RequestableDocumentId, builder);
        }



        #endregion


        #region "ConfirmDelete Request Document Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__GO_TO)]
        public ActionResult GoToConfirmDeleteRequestableDocuments()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_DELETE_AD_GET_PARAM_KEY];

            var confirmDeleteModel = (ConfirmDeleteRequestableDocumentModel)TempData[TEMP_DATA_CONFIRM_DELETE_MODEL_KEY];

            return View(GetConfiguredConfigureDeleteRequestableDocumentModel(account, adGetParam, confirmDeleteModel));
        }


        private ConfirmDeleteRequestableDocumentModel GetConfiguredConfigureDeleteRequestableDocumentModel(Account account, AdvancedGetParameters adGetParam = null, ConfirmDeleteRequestableDocumentModel confirmDeleteModel = null)
        {
            if (confirmDeleteModel == null)
            {
                confirmDeleteModel = new ConfirmDeleteRequestableDocumentModel();
            }
            confirmDeleteModel.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }


            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQUESTABLE_DOCUMENTS] = selectedIds;
            confirmDeleteModel.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ConfirmDeleteRequestableDocumentModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var entities = new List<RequestableDocument>();

            for (var i = 0; selectedIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var entity = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(selectedIds[i]);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            confirmDeleteModel.EntitiesInPage = entities;

            if (confirmDeleteModel.EntityRepresentationsInPage == null)
            {
                confirmDeleteModel.EntityRepresentationsInPage = new List<RequestableDocumentRepresentation>();
            }
            else
            {
                confirmDeleteModel.EntityRepresentationsInPage.Clear();
            }


            foreach (RequestableDocument ent in entities)
            {
                var entityRep = new RequestableDocumentRepresentation();
                entityRep.Id = ent.Id;
                entityRep.Title = ent.DocumentName;
                entityRep.ContentPreview = ent.GetNoteDescriptionAsString();
                entityRep.IsSelected = confirmDeleteModel.SelectedEntityIds.Contains(ent.Id);

                confirmDeleteModel.EntityRepresentationsInPage.Add(entityRep);
            }


            //

            confirmDeleteModel.TotalEntityCount = selectedIds.Count;

            if (!confirmDeleteModel.IsPageIndexValid(confirmDeleteModel.CurrentPageIndex))
            {
                confirmDeleteModel.CurrentPageIndex = 1;
            }

            return confirmDeleteModel;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__EXECUTE_ACTION)]
        public ActionResult ConfirmDeletePage_RequestableDocumentExecuteAction(ConfirmDeleteRequestableDocumentModel model, string executeAction)
        {
            if (executeAction.Equals("ConfirmDelete"))
            {
                return ConfirmDeleteRequestableDocumentsPage_DeleteSelected(model);
            }
            else if (executeAction.Equals("CancelDelete"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
            }
            else
            {
                return ConfirmDeleteRequestableDocumentsPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ConfirmDeleteRequestableDocumentsPage_DeleteSelected(ConfirmDeleteRequestableDocumentModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredConfigureDeleteRequestableDocumentModel(loggedInAccount, null, model);


            var idsToRemove = new List<int>();

            foreach (int id in model.SelectedEntityIds)
            {
                var success = annAccessor.AnnouncementManagerHelper.TryDeleteAnnouncementWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            //TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }


        private ActionResult ConfirmDeleteRequestableDocumentsPage_DoPageIndexChange(ConfirmDeleteRequestableDocumentModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = ManageAnnouncementsModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_DELETE_AD_GET_PARAM_KEY, adGetParam);

            //return GoToManageStudentsPage(model, adGetParam);
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }





        #endregion


        //


        #region "Manage Account Divisions Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO)]
        public ActionResult GoToManageDivisionsPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageAccountDivisionModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, GetConfiguredManageAccountDivisionModel(account, adGetParam, model));
        }

        private ManageAccountDivisionModel GetConfiguredManageAccountDivisionModel(Account account, AdvancedGetParameters adGetParam = null, ManageAccountDivisionModel model = null)
        {
            if (model == null)
            {
                model = new ManageAccountDivisionModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (CategoryRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageRequestableDocumentsModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageRequestableDocumentsModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            adGetParam.TextToContain = model.NameFilter;

            model.AdGetParam = adGetParam;

            //

            var entities = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(adGetParam);
            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<CategoryRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Category ent in entities)
            {
                var rep = new CategoryRepresentation();
                rep.Id = ent.Id;
                rep.Name = ent.Name;
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            model.TotalEntityCount = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoryCount(countAdGetParam);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageAccountDivisionPage_ExecuteAction(ManageAccountDivisionModel model, string executeAction)
        {
            if (executeAction.Equals("EditAction"))
            {
                return TransitionFromAccountDivisionManagePage_ToEditAccountDivisionPage(model);
            }
            else if (executeAction.Equals("DeleteAction"))
            {
                //return DeleteSelectedAdmins(model);
                return TransitionFromManagePage_ToDeleteAccountDivisionPage(model);
            }
            else if (executeAction.Equals("CreateAction"))
            {
                return Go_FromManageRequestableDocumentPage_ToCreateReqDocuPage();
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_ToManageAccountDivisionModel_ThenGoToManagePage(model);
            }
            else
            {
                return ManageAccountDivisionPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageAccountDivisionPage_DoPageIndexChange(ManageAccountDivisionModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }


        //When changing this, change the method in the BaseControllerWithNavBar as well.
        private ActionResult Go_FromManageAccountDivisionPage_ToCreateAccountDivisionPage()
        {
            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }


        private ActionResult SetTextFilter_ToManageAccountDivisionModel_ThenGoToManagePage(ManageAccountDivisionModel model)
        {
            model.CurrentPageIndex = 1;
            var selectedAccIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION];
            if (selectedAccIds != null)
            {
                selectedAccIds.Clear();
            }

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromAccountDivisionManagePage_ToEditAccountDivisionPage(ManageAccountDivisionModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAccountDivisionModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                RequestableDocument reqDocu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(selectedIds[0]);

                if (reqDocu != null)
                {
                    TempData.Add(TEMP_DATA_EDIT_ACCOUNT_DIVISION_KEY, reqDocu);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToDeleteAccountDivisionPage(ManageAccountDivisionModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAccountDivisionModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Category accDiv = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromId(selectedIds[0]);

                if (accDiv != null)
                {
                    TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }

        #endregion


        #region "Create/Edit Account Division"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__GO_TO)]
        public ActionResult GoToCreateEditAccountDivisionPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (CreateEditAccountDivisionModel)TempData[TEMP_DATA_MODEL_KEY];

            var accDivToEdit = (Category)TempData[TEMP_DATA_EDIT_ACCOUNT_DIVISION_KEY];

            return View(GetConfiguredCreateEditAccountDivisionModel(account, model, accDivToEdit));
        }


        private CreateEditAccountDivisionModel GetConfiguredCreateEditAccountDivisionModel(Account account, CreateEditAccountDivisionModel model, Category accountDivToEdit = null)
        {
            if (model == null)
            {
                model = new CreateEditAccountDivisionModel();
            }
            model.LoggedInAccount = account;

            //Supplied account to edit from manage page.
            if (accountDivToEdit != null)
            {
                model.AccountDivisionId = accountDivToEdit.Id;
                model.InputName = accountDivToEdit.Name;
                
            }

            //Supplied account type to create from manage page.
            else if (accountDivToEdit == null)
            {

            }

            //

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION)]
        public ActionResult ExecuteAction_CreateEditAccountDivision(CreateEditAccountDivisionModel model, string executeAction)
        {
            if (executeAction != null && executeAction.Equals("SaveChanges"))
            {
                return ActionSaveAccountDivision(model);
            }
            else
            {
                return Content(executeAction); //error
            }
        }

        private ActionResult ActionSaveAccountDivision(CreateEditAccountDivisionModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.IsActionCreateAccountDivision())
                    {
                        var accDivIdCreated = AttemptCreateAccountDivision(model);
                        var success = accDivIdCreated != -1;

                        if (success)
                        {
                            model.StatusMessage = "Account Division created!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                            model.AccountDivisionId = accDivIdCreated;

                            var accDivToEdit = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(accDivIdCreated);

                            TempData.Add(TEMP_DATA_EDIT_ACCOUNT_DIVISION_KEY, accDivToEdit);
                        }
                        else
                        {
                            model.StatusMessage = "Account Division creation failed!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                    }
                    else if (model.IsActionCreateAccountDivision())
                    {
                        var success = AttemptEditAccountDivision(model);

                        if (success)
                        {
                            model.StatusMessage = "Account Division edited!";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;

                        }
                        else
                        {
                            model.StatusMessage = "Error in editing. Please try again.";
                            model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                        }

                        TempData.Add(TEMP_DATA_EDIT_ACCOUNT_DIVISION_KEY, accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromId(model.AccountDivisionId));
                    }
                    else
                    {
                        model.StatusMessage = "Error: should not reach here!";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                catch (SqlException e)
                {
                    model.StatusMessage = string.Format("An error has occured. Please try again. {0}.", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (InputStringConstraintsViolatedException e)
                {
                    model.StatusMessage = String.Format("An invalid character has been detected: {0}", e.ViolatingString);
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
                catch (Exception e)
                {
                    model.StatusMessage = string.Format("A generic error has occured. Please try again. {0}", e.ToString());
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }

            }
            else
            {
                model.StatusMessage = "Invalid inputs detected";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CREATE_EDIT_REQUESTABLE_DOCUMENTS_PAGE__GO_TO, "Admin");
        }



        private int AttemptCreateAccountDivision(CreateEditAccountDivisionModel model)
        {
            var builder = new Category.Builder();
            builder.Name = model.InputName;
            
            var accDivId = accDivCategoryAccessor.CategoryDatabaseManagerHelper.CreateCategory(builder);

            return accDivId;
        }

        private bool AttemptEditAccountDivision(CreateEditAccountDivisionModel model)
        {
            var builder = new Category.Builder();
            builder.Name = model.InputName;

            //

            return accDivCategoryAccessor.CategoryDatabaseManagerHelper.EditCategory(model.AccountDivisionId, builder);
        }



        #endregion


        #region "ConfirmDelete Account Divisions Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__GO_TO)]
        public ActionResult GoToConfirmDeleteAccountDivision()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_DELETE_AD_GET_PARAM_KEY];

            var confirmDeleteModel = (ConfirmDeleteAccountDivisionModel)TempData[TEMP_DATA_CONFIRM_DELETE_MODEL_KEY];

            return View(GetConfiguredConfigureDeleteAccountDivisionModel(account, adGetParam, confirmDeleteModel));
        }


        private ConfirmDeleteAccountDivisionModel GetConfiguredConfigureDeleteAccountDivisionModel(Account account, AdvancedGetParameters adGetParam = null, ConfirmDeleteAccountDivisionModel confirmDeleteModel = null)
        {
            if (confirmDeleteModel == null)
            {
                confirmDeleteModel = new ConfirmDeleteAccountDivisionModel();
            }
            confirmDeleteModel.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }


            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACCOUNT_DIVISION] = selectedIds;
            confirmDeleteModel.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ConfirmDeleteRequestableDocumentModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var entities = new List<Category>();

            for (var i = 0; selectedIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var entity = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromId(selectedIds[i]);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            confirmDeleteModel.EntitiesInPage = entities;

            if (confirmDeleteModel.EntityRepresentationsInPage == null)
            {
                confirmDeleteModel.EntityRepresentationsInPage = new List<CategoryRepresentation>();
            }
            else
            {
                confirmDeleteModel.EntityRepresentationsInPage.Clear();
            }


            foreach (Category ent in entities)
            {
                var entityRep = new CategoryRepresentation();
                entityRep.Id = ent.Id;
                entityRep.Name = ent.Name;
                entityRep.IsSelected = confirmDeleteModel.SelectedEntityIds.Contains(ent.Id);

                confirmDeleteModel.EntityRepresentationsInPage.Add(entityRep);
            }


            //

            confirmDeleteModel.TotalEntityCount = selectedIds.Count;

            if (!confirmDeleteModel.IsPageIndexValid(confirmDeleteModel.CurrentPageIndex))
            {
                confirmDeleteModel.CurrentPageIndex = 1;
            }

            return confirmDeleteModel;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__EXECUTE_ACTION)]
        public ActionResult ConfirmDeletePage_AccountDivisionExecuteAction(ConfirmDeleteAccountDivisionModel model, string executeAction)
        {
            if (executeAction.Equals("ConfirmDelete"))
            {
                return ConfirmDeleteAccountDivisionPage_DeleteSelected(model);
            }
            else if (executeAction.Equals("CancelDelete"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
            }
            else
            {
                return ConfirmDeleteAccountDivisionPage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ConfirmDeleteAccountDivisionPage_DeleteSelected(ConfirmDeleteAccountDivisionModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredConfigureDeleteAccountDivisionModel(loggedInAccount, null, model);


            var idsToRemove = new List<int>();

            foreach (int id in model.SelectedEntityIds)
            {
                var success = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryDeleteCategoryWithId(id);
                if (success)
                {
                    idsToRemove.Add(id);
                }
            }

            foreach (int id in idsToRemove)
            {
                model.SelectedEntityIds.Remove(id);
            }

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }


        private ActionResult ConfirmDeleteAccountDivisionPage_DoPageIndexChange(ConfirmDeleteAccountDivisionModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = ManageAnnouncementsModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_CONFIRM_DELETE_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_DELETE_AD_GET_PARAM_KEY, adGetParam);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__DELETE_SELECTED_ACCOUNT_DIVISION_PAGE__GO_TO, "Admin");
        }





        #endregion


        //

        #region "Configure Account's Permissions Page"


        [ActionName(ActionNameConstants.ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__GO_TO)]
        public ActionResult GoToConfigureAccountPermissionsPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ConfigurePermissionsOfAccountModel)TempData[TEMP_DATA_MODEL_KEY];

            var accountPermissionToEdit = (Account)TempData[TEMP_DATA_EDIT_PERMISSION_OF_ACCOUNT_KEY];

            return View(GetConfiguredConfigureAccountPermissionsModel(account, model, accountPermissionToEdit));
        }

        private ConfigurePermissionsOfAccountModel GetConfiguredConfigureAccountPermissionsModel(Account account, ConfigurePermissionsOfAccountModel model, Account accountPermissionToEdit = null)
        {
            
            if (model == null)
            {
                model = new ConfigurePermissionsOfAccountModel();
            }
            model.LoggedInAccount = account;

            
            //Supplied account to edit from manage page.
            if (accountPermissionToEdit != null)
            {
                var adGetParamForPerm = new AdvancedGetParameters();

                var allPermissions = permissionAccessor.PermissionDatabaseManagerHelper.AdvancedGetPermissionsAsList(adGetParamForPerm);
                
                var permissionIdsOfAccount = accountToPermissionsAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(accountPermissionToEdit.Id, adGetParamForPerm);

                //

                if (model.AllPermissionRepresentation == null)
                {
                    model.AllPermissionRepresentation = new List<PermissionRepresentation>();
                }
                else
                {
                    model.AllPermissionRepresentation.Clear();
                }

                foreach (Permission perm in allPermissions)
                {
                    var permRep = new PermissionRepresentation();
                    permRep.Id = perm.Id;
                    permRep.Title = perm.Name;
                    permRep.FullContent = perm.GetDescriptionAsString();
                    permRep.IsSelected = permissionIdsOfAccount.Contains(perm.Id);

                    model.AllPermissionRepresentation.Add(permRep);
                }


                //

                model.AccPermissionToEditId = accountPermissionToEdit.Id;
                model.AccPermissionToEditUsername = accountPermissionToEdit.Username;
            }
            

            //

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__EXECUTE_ACTION)]
        public ActionResult ConfigurePermissionsPage_ExecuteAction(ConfigurePermissionsOfAccountModel model, string executeAction)
        {
            if (executeAction.Equals("SaveChanges"))
            {
                return AttemptConfigurePermissionOfAccount(model);
            }
            else if (executeAction.Equals("CancelChanges"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
            }
            else
            {
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ADMIN_PAGE__GO_TO, "Admin");
            }
        }


        private ActionResult AttemptConfigurePermissionOfAccount(ConfigurePermissionsOfAccountModel model)
        {
            var accIdToModify = model.AccPermissionToEditId;
            var selectedPermissionIds = new List<int>();
            foreach (PermissionRepresentation permRep in model.AllPermissionRepresentation)
            {
                if (permRep.IsSelected)
                {
                    selectedPermissionIds.Add(permRep.Id);
                }
            }


            try
            {
                if (accountToPermissionsAccessor.EntityToCategoryDatabaseManagerHelper.IfPrimaryExsists(accIdToModify)) 
                {
                    accountToPermissionsAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(accIdToModify);
                }

                foreach (int permId in selectedPermissionIds)
                {
                    accountToPermissionsAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(accIdToModify, permId);
                }

                model.StatusMessage = "Perissions of account edited!";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
            }
            catch (Exception)
            {
                model.StatusMessage = "A generic error occurred!";
                model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
            }


            TempData[TEMP_DATA_MODEL_KEY] = model;
            TempData[TEMP_DATA_EDIT_PERMISSION_OF_ACCOUNT_KEY] = accAccessor.AccountDatabaseManagerHelper.GetAccountInfoFromId(model.AccPermissionToEditId);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__CONFIGURE_PERMISSIONS_OF_ACCOUNT__GO_TO, "Admin");
        }

        #endregion


        //


        #region "Manage Req Docu Queue Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult GoToManageReqDocuQueuePage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageReqDocuQueueModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, GetConfiguredManageReqDocuQueueModel(account, adGetParam, model));
        }

        private ManageReqDocuQueueModel GetConfiguredManageReqDocuQueueModel(Account account, AdvancedGetParameters adGetParam = null, ManageReqDocuQueueModel model = null)
        {
            if (model == null)
            {
                model = new ManageReqDocuQueueModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (QueueRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

           
            model.QueueStatusList = new List<String>();
            
            if (model.IfAccountHasPermission(PermissionConstants.MANAGE_QUEUE_FOR_REQUESTABLE_DOCUMENTS))
            {
                model.QueueStatusList.Add(QueueStatusConstants.GetStatusAsText(QueueStatusConstants.STATUS_PENDING));
            }
            if (model.IfAccountHasPermission(PermissionConstants.MANAGE_HISTORY_OF_REQUESTED_DOCUMENT))
            {
                model.QueueStatusList.Add(QueueStatusConstants.GetStatusAsText(QueueStatusConstants.STATUS_COMPLETED));
                model.QueueStatusList.Add(QueueStatusConstants.GetStatusAsText(QueueStatusConstants.STATUS_TERMINATED));
            }


            if (model.SelectedQueueStatus == null)
            {
                model.SelectedQueueStatus = model.QueueStatusList[0]; //Will always have at least 1
            }

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageReqDocuQueueModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageReqDocuQueueModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            //adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            //var entities = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);
            currentAdGetParam = adGetParam;
            currentLoggedInAccount = model.LoggedInAccount;
            currentQueueAdGetParam = new QueueAdvancedGetParameters();
            currentQueueAdGetParam.StatusFilter = QueueStatusConstants.GetStatusAsInt(model.SelectedQueueStatus);

            var reqDocuQueueIdsInDivRespoOfLoggedIn = reqDocuQueueToAccAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedRelationsSatisfyingCondition(adGetParam,
                IsQueueAssociatedAccInAllDivisionResponsibilityOfLoggedIn_AndSatisfyQueueAdGetParam,
                false
                );
            var entities = new List<Queue>();
            foreach (Relation rel in reqDocuQueueIdsInDivRespoOfLoggedIn)
            {
                entities.Add(reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(rel.PrimaryId));
            }



            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<QueueRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Queue ent in entities)
            {
                var rep = new QueueRepresentation();
                rep.Id = ent.Id;
                rep.DescriptionPreview = ent.GetDescriptionAsString();
                rep.Status = ent.Status;
                rep.StatusAsText = QueueStatusConstants.GetStatusAsText(rep.Status);
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                var reqDocuIdsAssociated = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(ent.Id, new AdvancedGetParameters());
                if (reqDocuIdsAssociated.Count == 1)
                {
                    var reqDocuId = -1;
                    foreach (int id in reqDocuIdsAssociated)
                    {
                        reqDocuId = id;
                        break;
                    }
                    var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(reqDocuId);
                    if (docu != null)
                    {
                        rep.AssociatedReqDocuName = docu.DocumentName;
                    }
                }

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;

            //model.TotalEntityCount = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementCount(countAdGetParam);
            currentAdGetParam = countAdGetParam;
            currentQueueAdGetParam = new QueueAdvancedGetParameters();
            currentQueueAdGetParam.StatusFilter = QueueStatusConstants.GetStatusAsInt(model.SelectedQueueStatus);

            var reqDocuQueueCountInDivRespoOfLoggedIn = reqDocuQueueToAccAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationCountSatisfyingCondition(adGetParam,
                IsQueueAssociatedAccInAllDivisionResponsibilityOfLoggedIn_AndSatisfyQueueAdGetParam,
                false
                );
            model.TotalEntityCount = reqDocuQueueCountInDivRespoOfLoggedIn;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }

        

        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION)]
        public ActionResult ManageReqDocuQueuePage_ExecuteAction(ManageReqDocuQueueModel model, string executeAction)
        {
            if (executeAction == null)
            {
                TempData.Add(TEMP_DATA_MODEL_KEY, model);

                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, ControllerNameConstants.ADMIN_CONTROLLER_NAME);
            }
            else
            {
                if (executeAction.Equals("InspectAction"))
                {
                    return TransitionFromReqDocuQueueManagePage_ToInspectActionPage(model);
                }
                else if (executeAction.Equals("TerminateAction"))
                {
                    return TransitionFromManagePage_ToTerminateReqDocuQueuePage(model);
                }
                else
                {
                    return ManageReqDocuQueuePage_DoPageIndexChange(model, executeAction);
                }
            }
            
        }

        private ActionResult ManageReqDocuQueuePage_DoPageIndexChange(ManageReqDocuQueueModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromReqDocuQueueManagePage_ToInspectActionPage(ManageReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageReqDocuQueueModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Queue queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[0]);

                if (queue != null)
                {
                    //TempData.Add(TEMP_DATA_TAKE_ACTION_REQ_DOCU_QUEUE_KEY, queue);
                    //TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToTerminateReqDocuQueuePage(ManageReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageReqDocuQueueModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Queue queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[0]);
                
                if (queue != null)
                {
                    //TempData.Add(TEMP_DATA_MODEL_KEY, model);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }

        #endregion


        #region "Inspect Selected Req Docu Queue Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult GoToInspectSelectedReqDocuPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            var model = (InspectReqDocuQueueModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfiguredInspectReqDocuQueueModel(account, adGetParam, model));
        }


        private InspectReqDocuQueueModel GetConfiguredInspectReqDocuQueueModel(Account account, AdvancedGetParameters adGetParam = null, InspectReqDocuQueueModel model = null)
        {
            if (model == null)
            {
                model = new InspectReqDocuQueueModel();
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }


            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = InspectReqDocuQueueModel.ENTITIES_PER_PAGE;
            model.AdGetParam = adGetParam;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var entities = new List<Queue>();

            for (var i = 0; selectedIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var entity = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[i]);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<QueueRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }


            foreach (Queue ent in entities)
            {
                var rep = new QueueRepresentation();
                rep.Id = ent.Id;
                rep.FullDescription = ent.GetDescriptionAsString();
                rep.Status = ent.Status;
                rep.StatusAsText = QueueStatusConstants.GetStatusAsText(rep.Status);
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                var reqDocuIdsAssociated = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(ent.Id, new AdvancedGetParameters());
                if (reqDocuIdsAssociated.Count == 1)
                {
                    var reqDocuId = -1;
                    foreach (int id in reqDocuIdsAssociated)
                    {
                        reqDocuId = id;
                        break;
                    }
                    var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(reqDocuId);
                    if (docu != null)
                    {
                        rep.AssociatedReqDocuName = docu.DocumentName;
                    }
                }

                model.EntityRepresentationsInPage.Add(rep);
            }


            //

            model.TotalEntityCount = selectedIds.Count;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION)]
        public ActionResult InspectSelectedReqDocuQueue_ExecuteAction(InspectReqDocuQueueModel model, string executeAction)
        {
            if (executeAction.Equals("TakeAction"))
            {
                return InspectSelectedReqDocuQueuePage_TakeOn(model);
            }
            else if (executeAction.Equals("CancelAction"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
            }
            else
            {
                return InspectReqDocuQueue_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult InspectSelectedReqDocuQueuePage_TakeOn(InspectReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredInspectReqDocuQueueModel(loggedInAccount, null, model);


            var idsToTakeOn = model.SelectedEntityIds;

            foreach (int id in idsToTakeOn)
            {
                var queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(id);

                if (queue != null)
                {
                    var builder = queue.ConstructBuilderFromSelf();
                    builder.Status = QueueStatusConstants.STATUS_ACKNOWLEDGED;

                    try
                    {
                        var editSuccess = reqDocuQueueAccessor.QueueDatabaseManagerHelper.EditQueue(id, builder);

                        fulfillerAccToReqDocuQueueAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.LoggedInAccount.Id, queue.Id);

                        return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                    }
                    catch (Exception)
                    {
                        model.StatusMessage = "An error occurred! Please try again.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
            }

            TempData[TEMP_DATA_MODEL_KEY] = model;

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult InspectReqDocuQueue_DoPageIndexChange(InspectReqDocuQueueModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = InspectReqDocuQueueModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_AD_GET_PARM_KEY, adGetParam);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__INSPECT_SELECTED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }





        #endregion


        #region "Terminate Selected Req Docu Queue Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult GoToTerminateSelectedReqDocuPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            var model = (TerminateReqDocuQueueModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfiguredTerminateReqDocuQueueModel(account, adGetParam, model));
        }


        private TerminateReqDocuQueueModel GetConfiguredTerminateReqDocuQueueModel(Account account, AdvancedGetParameters adGetParam = null, TerminateReqDocuQueueModel model = null)
        {
            if (model == null)
            {
                model = new TerminateReqDocuQueueModel();
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }


            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_REQ_DOCU_QUEUE] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = InspectReqDocuQueueModel.ENTITIES_PER_PAGE;

            //

            var currentOffsetCount = adGetParam.Offset;
            var currentFetchCount = adGetParam.Fetch;

            var hasFetch = adGetParam.Fetch != 0;
            var entities = new List<Queue>();

            for (var i = 0; selectedIds.Count > i; i++)
            {
                if (currentOffsetCount > 0)
                {
                    currentOffsetCount -= 1;
                    continue;
                }

                if (currentFetchCount > 0 || !hasFetch)
                {
                    var entity = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[i]);
                    if (entity != null)
                    {
                        entities.Add(entity);
                    }


                    currentFetchCount -= 1;
                }

                if (currentFetchCount <= 0 && hasFetch)
                {
                    break;
                }
            }

            //

            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<QueueRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }


            foreach (Queue ent in entities)
            {
                var rep = new QueueRepresentation();
                rep.Id = ent.Id;
                rep.FullDescription = ent.GetDescriptionAsString();
                rep.Status = ent.Status;
                rep.StatusAsText = QueueStatusConstants.GetStatusAsText(rep.Status);
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                var reqDocuIdsAssociated = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(ent.Id, new AdvancedGetParameters());
                if (reqDocuIdsAssociated.Count == 1)
                {
                    var reqDocuId = -1;
                    foreach (int id in reqDocuIdsAssociated)
                    {
                        reqDocuId = id;
                        break;
                    }
                    var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(reqDocuId);
                    if (docu != null)
                    {
                        rep.AssociatedReqDocuName = docu.DocumentName;
                    }
                }

                model.EntityRepresentationsInPage.Add(rep);
            }


            //

            model.TotalEntityCount = selectedIds.Count;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult TerminateSelectedReqDocuQueue_ExecuteAction(TerminateReqDocuQueueModel model, string executeAction)
        {
            if (executeAction.Equals("TerminateAction"))
            {
                return TerminateSelectedReqDocuQueuePage_Terminate(model);
            }
            else if (executeAction.Equals("CancelAction"))
            {
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
            }
            else
            {
                return TerminateReqDocuQueue_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult TerminateSelectedReqDocuQueuePage_Terminate(TerminateReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredTerminateReqDocuQueueModel(loggedInAccount, null, model);


            var idsToTakeOn = model.SelectedEntityIds;

            foreach (int id in idsToTakeOn)
            {
                var queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(id);

                if (queue != null)
                {
                    var builder = queue.ConstructBuilderFromSelf();
                    builder.Status = QueueStatusConstants.STATUS_TERMINATED;

                    try
                    {
                        var editSuccess = reqDocuQueueAccessor.QueueDatabaseManagerHelper.EditQueue(id, builder);

                        return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                    }
                    catch (Exception)
                    {
                        model.StatusMessage = "An error occurred! Please try again.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
            }

            TempData[TEMP_DATA_MODEL_KEY] = model;

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TerminateReqDocuQueue_DoPageIndexChange(TerminateReqDocuQueueModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);

            var adGetParam = new AdvancedGetParameters();
            adGetParam.Offset = TerminateReqDocuQueueModel.GetOffsetToUseBasedOnPageIndex(pageIndexSelected);


            TempData.Add(TEMP_DATA_MODEL_KEY, model);
            TempData.Add(TEMP_DATA_AD_GET_PARM_KEY, adGetParam);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__TERMINATE_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }





        #endregion


        //


        #region "Manage Acknowledged Req Docu Queue Table page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult GoToManageAcknowledgedReqDocuQueuePage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var model = (ManageAcknowledgedReqDocuQueueModel)TempData[TEMP_DATA_MODEL_KEY];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];

            return View(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, GetConfiguredManageAcknowledgedReqDocuQueueModel(account, adGetParam, model));
        }

        private ManageAcknowledgedReqDocuQueueModel GetConfiguredManageAcknowledgedReqDocuQueueModel(Account account, AdvancedGetParameters adGetParam = null, ManageAcknowledgedReqDocuQueueModel model = null)
        {
            if (model == null)
            {
                model = new ManageAcknowledgedReqDocuQueueModel(account);
            }
            model.LoggedInAccount = account;

            //

            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE];
            if (selectedIds == null)
            {
                selectedIds = new List<int>();
            }

            if (model.EntityRepresentationsInPage != null)
            {
                foreach (QueueRepresentation rep in model.EntityRepresentationsInPage)
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

            System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE] = selectedIds;
            model.SelectedEntityIds = selectedIds;

            //

            if (adGetParam == null)
            {
                adGetParam = new AdvancedGetParameters();
            }
            adGetParam.Fetch = ManageAcknowledgedReqDocuQueueModel.ENTITIES_PER_PAGE;
            adGetParam.Offset = ManageAcknowledgedReqDocuQueueModel.GetOffsetToUseBasedOnPageIndex(model.CurrentPageIndex);
            //adGetParam.TextToContain = model.TitleFilter;

            model.AdGetParam = adGetParam;

            //

            var reqDocuQueueIdsAcknowledgedLoggedIn = fulfillerAccToReqDocuQueueAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.LoggedInAccount.Id, adGetParam);
            var entities = new List<Queue>();
            foreach (int queueId in reqDocuQueueIdsAcknowledgedLoggedIn)
            {
                entities.Add(reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(queueId));
            }



            model.EntitiesInPage = entities;

            if (model.EntityRepresentationsInPage == null)
            {
                model.EntityRepresentationsInPage = new List<QueueRepresentation>();
            }
            else
            {
                model.EntityRepresentationsInPage.Clear();
            }

            foreach (Queue ent in entities)
            {
                var rep = new QueueRepresentation();
                rep.Id = ent.Id;
                rep.DescriptionPreview = ent.GetDescriptionAsString();
                rep.Status = ent.Status;
                rep.StatusAsText = QueueStatusConstants.GetStatusAsText(rep.Status);
                rep.IsSelected = model.SelectedEntityIds.Contains(ent.Id);

                var reqDocuIdsAssociated = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(ent.Id, new AdvancedGetParameters());
                if (reqDocuIdsAssociated.Count == 1)
                {
                    var reqDocuId = -1;
                    foreach (int id in reqDocuIdsAssociated)
                    {
                        reqDocuId = id;
                        break;
                    }
                    var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(reqDocuId);
                    if (docu != null)
                    {
                        rep.AssociatedReqDocuName = docu.DocumentName;
                    }
                }

                model.EntityRepresentationsInPage.Add(rep);
            }

            //

            var countAdGetParam = new AdvancedGetParameters();
            countAdGetParam.TextToContain = adGetParam.TextToContain;
            countAdGetParam.OrderByParameters = adGetParam.OrderByParameters;


            var reqDocuQueueCountAcknowledgedLoggedIn = reqDocuQueueToAccAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationOfPrimaryCount(model.LoggedInAccount.Id, countAdGetParam);
            model.TotalEntityCount = reqDocuQueueCountAcknowledgedLoggedIn;

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }


        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult ManageAcknowledgedReqDocuQueuePage_ExecuteAction(ManageAcknowledgedReqDocuQueueModel model, string executeAction)
        {
            if (executeAction.Equals("FulfillAction"))
            {
                return TransitionFromManageAcknowledgedReqDocuQueueManagePage_ToFulfillActionPage(model);
            }
            else if (executeAction.Equals("TerminateAction"))
            {
                return TransitionFromManagePage_ToTerminateAcknowledgedReqDocuQueuePage(model);
            }
            else
            {
                return ManageAcknowledgedReqDocuQueuePage_DoPageIndexChange(model, executeAction);
            }
        }

        private ActionResult ManageAcknowledgedReqDocuQueuePage_DoPageIndexChange(ManageAcknowledgedReqDocuQueueModel model, string pageIndex)
        {
            var pageIndexSelected = int.Parse(pageIndex);
            model.CurrentPageIndex = pageIndexSelected;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManageAcknowledgedReqDocuQueueManagePage_ToFulfillActionPage(ManageAcknowledgedReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAcknowledgedReqDocuQueueModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Queue queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[0]);

                if (queue != null)
                {
                    TempData.Add(TEMP_DATA_FULFILL_ACKNOWLEDGED_REQ_DOCU_KEY, queue);
                    
                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private ActionResult TransitionFromManagePage_ToTerminateAcknowledgedReqDocuQueuePage(ManageAcknowledgedReqDocuQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredManageAcknowledgedReqDocuQueueModel(loggedInAccount, null, model);
            var selectedIds = (IList<int>)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_SELECTED_ACKNOWLEDGED_REQ_DOCU_QUEUE];

            if (selectedIds != null && selectedIds.Count > 0)
            {
                Queue queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(selectedIds[0]);

                if (queue != null)
                {
                    //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                    TempData.Add(TEMP_DATA_TERMINATE_ACKNOWLEDGED_REQ_DOCU_KEY, queue);

                    return RedirectToAction(ActionNameConstants.ADMIN_SIDE__TERMINATE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                }
            }

            //

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }

        #endregion


        #region "Fulfill Req Docu Queue Page"

        [ActionName(ActionNameConstants.ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO)]
        public ActionResult GoToFulfillAcknowledgedReqDocuPage()
        {
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            //var model = (ManageStudentAccountsModel)TempData[TEMP_DATA_MODEL_KEY];
            var queue = (Queue)TempData[TEMP_DATA_FULFILL_ACKNOWLEDGED_REQ_DOCU_KEY];

            var model = (FulfillAcknowledgedReqDocQueueModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfiguredFulfillAcknowledgedReqDocuQueueModel(account, queue, model));
        }


        private FulfillAcknowledgedReqDocQueueModel GetConfiguredFulfillAcknowledgedReqDocuQueueModel(Account account, Queue queueToFulfill, FulfillAcknowledgedReqDocQueueModel model = null)
        {
            if (model == null)
            {
                model = new FulfillAcknowledgedReqDocQueueModel();
            }
            model.LoggedInAccount = account;

            //

            if (queueToFulfill != null)
            {
                var rep = new QueueRepresentation();
                rep.Id = queueToFulfill.Id;
                rep.FullDescription = queueToFulfill.GetDescriptionAsString();
                rep.Status = queueToFulfill.Status;
                rep.StatusAsText = QueueStatusConstants.GetStatusAsText(rep.Status);
                
                var reqDocuIdsAssociated = reqDocuQueueToReqDocuAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(queueToFulfill.Id, new AdvancedGetParameters());
                if (reqDocuIdsAssociated.Count == 1)
                {
                    var reqDocuId = -1;
                    foreach (int id in reqDocuIdsAssociated)
                    {
                        reqDocuId = id;
                        break;
                    }
                    var docu = requestDocuAccessor.ReqDocuManagerHelper.TryGetRequestableDocumentFromId(reqDocuId);
                    if (docu != null)
                    {
                        rep.AssociatedReqDocuName = docu.DocumentName;
                    }
                }

                model.QueueRep = rep;
            }

            //



            return model;
        }



        [HttpPost]
        [ActionName(ActionNameConstants.ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__EXECUTE_ACTION)]
        public ActionResult FulfillAcknowledgedReqDocuQueue_ExecuteAction(FulfillAcknowledgedReqDocQueueModel model, string executeAction)
        {
            if (executeAction.Equals("FulfillAction"))
            {
                return AcknowledgedReqDocuQueuePage_Fulfill(model);
            }
            else if (executeAction.Equals("CancelAction"))
            {
                //TempData.Add(TEMP_DATA_MODEL_KEY, model);
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
            }
            else
            {
                return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
            }
        }

        private ActionResult AcknowledgedReqDocuQueuePage_Fulfill(FulfillAcknowledgedReqDocQueueModel model)
        {
            var loggedInAccount = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            model = GetConfiguredFulfillAcknowledgedReqDocuQueueModel(loggedInAccount, null, model);


            var queue = reqDocuQueueAccessor.QueueDatabaseManagerHelper.TryGetQueueInfoFromId(model.QueueRep.Id);

            if (queue != null)
            {
                var builder = queue.ConstructBuilderFromSelf();
                builder.Status = QueueStatusConstants.STATUS_COMPLETED;

                try
                {
                    if (!model.IsCompleted || queue.Status == QueueStatusConstants.STATUS_COMPLETED)
                    {
                        var editSuccess = reqDocuQueueAccessor.QueueDatabaseManagerHelper.EditQueue(queue.Id, builder);
                        fulfillerAccToReqDocuQueueAccessor.EntityToCategoryDatabaseManagerHelper.DeleteEntityCategoryRelation(model.LoggedInAccount.Id, queue.Id);

                        NotifyAccountRequesterOfDocument(model);

                        model.IsCompleted = true;
                        model.StatusMessage = "Request fulfilled. Notification sent to sender.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_SUCCESS;
                        //return RedirectToAction(ActionNameConstants.ADMIN_SIDE__MANAGE_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
                    }
                    else
                    {
                        model.StatusMessage = "Request already fulfilled.";
                        model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                    }
                }
                catch (Exception)
                {
                    model.StatusMessage = "An error occurred! Please try again.";
                    model.ActionExecuteStatus = ActionStatusConstants.STATUS_FAILED;
                }
            }
            

            TempData[TEMP_DATA_MODEL_KEY] = model;

            return RedirectToAction(ActionNameConstants.ADMIN_SIDE__FULFILL_ACKNOWLEDGED_REQ_DOC_QUEUE_PAGE__GO_TO, "Admin");
        }


        private bool NotifyAccountRequesterOfDocument(FulfillAcknowledgedReqDocQueueModel model)
        {
            var requesterAccId = GetRequesterOfDocument(model.QueueRep.Id);

            if (requesterAccId != -1) {
                //TODO Create Notif, NOTIFY REQUESTER (attach relation of acc to notif), AND ATTACH RELATION of notification to document
                var notifBuilder = new Notification.Builder();
                notifBuilder.IsHighlighted = true;
                notifBuilder.Title = ConstructTitleForNotification(model);
                notifBuilder.SetContentWithString(ConstructDescriptionForNotification(model));
                notifBuilder.DateSent = DateTime.Now;

                var notifId = notificationAccessor.NotificationManagerHelper.CreateNotification(notifBuilder);

                //

                accountToNotificationAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(requesterAccId, notifId);

                //

                if (model.InputDocument != null && model.InputDocument.ContentLength != 0)
                {
                    MemoryStream target = new MemoryStream();
                    model.InputDocument.InputStream.Position = 0;
                    model.InputDocument.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    

                    var docuBuilder = new Document.Builder();
                    docuBuilder.DirectoryPath = @"C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\Res\Documents";
                    docuBuilder.DocumentContentAsBytes = data;
                    docuBuilder.DocumentExtension = Path.GetExtension(model.InputDocument.FileName);
                    docuBuilder.OriginalNameOfFile = Path.GetFileNameWithoutExtension(model.InputDocument.FileName);

                    var docuId = documentFileAccessor.DocumentDatabaseManagerHelper.CreateDocument(docuBuilder);

                    if (docuId != -1)
                    {
                        notificationToDocumentAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(notifId, docuId);
                    }
                }

            }

            return true;
        }

        private int GetRequesterOfDocument(int queueId)
        {
            var accIds = reqDocuQueueToAccAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(queueId, new AdvancedGetParameters());

            foreach (int accId in accIds)
            {
                return accId;
            }

            return -1;
        }

        private string ConstructTitleForNotification(FulfillAcknowledgedReqDocQueueModel model)
        {
            return string.Format("Request for {0} is completed",
                    model.QueueRep.AssociatedReqDocuName
                    );
        }

        private string ConstructDescriptionForNotification(FulfillAcknowledgedReqDocQueueModel model)
        {
            var sBuilder = new StringBuilder();

            //

            if (!string.IsNullOrEmpty(model.InputFulfillerNotificationRemarks))
            {
                //var part2Text = "Additional notes from fulfiller:@&";
                //part2Text = part2Text.Replace("@&", System.Environment.NewLine);

                var part3Text = model.InputFulfillerNotificationRemarks;

                //sBuilder.Append(part2Text);
                sBuilder.Append(part3Text);
            }

            //

            return sBuilder.ToString();
        }


        #endregion


    }
}