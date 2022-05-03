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


            var announcements = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);
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

            model.TotalEntityCount = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementCount(countAdGetParam);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

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

            var adminAccs = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountsAsList(adGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);
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

            //TODO MAKE Account display based on departmental responsibility
            model.TotalEntityCount = accAccessor.AccountDatabaseManagerHelper.AdvancedGetAccountCount(countAdGetParam, TypeConstants.ACCOUNT_TYPE_NORMAL);

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

            //

            var adGetParamForCat = new AdvancedGetParameters();
            var catList = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryAdvancedGetCategoriesAsList(adGetParamForCat);

            model.CategoryNameList = new List<string>();

            foreach (Category cat in catList)
            {
                model.CategoryNameList.Add(cat.Name);
            }

            //

            model.InputCategoryNameList = new List<string>();

            var relList = accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AccountId, new AdvancedGetParameters());

            if (relList != null)
            {
                foreach (int id in relList)
                {
                    var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(id);
                    if (cat != null)
                    {
                        model.InputCategoryNameList.Add(cat.Name);

                        if (model.CategoryNameList.Contains(cat.Name))
                        {
                            model.CategoryNameList.Remove(cat.Name);
                        }
                    }
                }
            }

            //

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

            if (model.InputCategoryNameList != null)
            {
                foreach (string catName in model.InputCategoryNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(accId, accCat.Id);
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

            if (model.InputCategoryNameList != null)
            {
                foreach (string catName in model.InputCategoryNameList)
                {
                    var accCat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(catName);
                    if (accCat != null)
                    {
                        accToDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AccountId, accCat.Id);
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

            var entities = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);
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

            model.TotalEntityCount = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementCount(countAdGetParam);

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

            model.ListOfEmployeeCategoriesName = GetConfiguredListOfEmployeeCategoriesForChoices(model.ListOfEmployeeCategoriesName);
            model.ListOfEmployeeCategoriesName.Add(ANNOUNCE_TO_ALL_ITEM);

            
            var relList = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.AnnouncementId, new AdvancedGetParameters());
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
                            model.InputChosenEmployeeCategoryName = cat.Name;
                            break;
                        }
                    }
                }
                else
                {
                    model.InputChosenEmployeeCategoryName = ANNOUNCE_TO_ALL_ITEM;
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

            if (annId != -1)
            {
                if (!ANNOUNCE_TO_ALL_ITEM.Equals(model.InputChosenEmployeeCategoryName))
                {
                    var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(model.InputChosenEmployeeCategoryName);

                    if (cat != null)
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(annId, cat.Id);
                    }
                }
                else
                {
                    var allCats = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());

                    foreach (Category cat in allCats)
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(annId, cat.Id);
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

            
            var cat = accDivCategoryAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(model.InputChosenEmployeeCategoryName);

            if (!ANNOUNCE_TO_ALL_ITEM.Equals(model.InputChosenEmployeeCategoryName))
            {
                if (cat != null || CreateEditEmployeeAccountModel.NO_EMPLOYEE_DIV_CATEGORY_NAME.Equals(model.InputChosenEmployeeCategoryName))
                {
                    if (annToCatAccessor.EntityToCategoryDatabaseManagerHelper.IfPrimaryExsists(model.AnnouncementId))
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(model.AnnouncementId);
                    }

                    if (cat != null)
                    {
                        annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AnnouncementId, cat.Id);
                    }
                }
            }
            else
            {
                if (annToCatAccessor.EntityToCategoryDatabaseManagerHelper.IfPrimaryExsists(model.AnnouncementId))
                {
                    annToCatAccessor.EntityToCategoryDatabaseManagerHelper.DeletePrimaryWithId(model.AnnouncementId);
                }


                var allCats = accDivCategoryAccessor.CategoryDatabaseManagerHelper.AdvancedGetCategoriesAsList(new AdvancedGetParameters());

                foreach (Category empCat in allCats)
                {
                    annToCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(model.AnnouncementId, empCat.Id);
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


    }
}