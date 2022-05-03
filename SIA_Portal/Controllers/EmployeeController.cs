using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIA_Portal.Constants;
using SIA_Portal.Models.RegisNotRequired;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Accessors;
using SIA_Portal.Controllers;
using CommonDatabaseActionReusables.AccountManager;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.Models.EmployeeModel;
using SIA_Portal.Models.ObjectRepresentations;
using CommonDatabaseActionReusables.AnnouncementManager;
using CommonDatabaseActionReusables.CategoryManager;
using CommonDatabaseActionReusables.RelationManager;
using SIA_Portal.Utilities.DomIdentifier;

namespace SIA_Portal.Controllers
{

    public class EmployeeController : BaseControllers.BaseControllerWithNavBarController
    {

        private const string TEMP_DATA_MODEL_KEY = "TempDataModelKey";
        private const string TEMP_DATA_AD_GET_PARM_KEY = "AdGetParamKey";

        private PortalAnnouncementAccessor annAccessor = new PortalAnnouncementAccessor();

        private PortalAnnToCatAccessor annToCatAccessor = new PortalAnnToCatAccessor();
        private PortalAccountDivisionCategoryAccessor employeeDivCategoryAccessor = new PortalAccountDivisionCategoryAccessor();
        private PortalAccountToAccDivCatAccessor accToEmployeeDivCatAccessor = new PortalAccountToAccDivCatAccessor();

        #region "Home Page"


        [ActionName(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE)]
        public ActionResult Index()
        {
            //var account = TempData[ControllerConstants.TEMP_ACCOUNT_OBJ];
            var account = (Account)System.Web.HttpContext.Current.Session[SessionConstants.SESSION_ACCOUNT_OBJ];
            var adGetParam = (AdvancedGetParameters)TempData[TEMP_DATA_AD_GET_PARM_KEY];
            var model = (EmployeeHomePageModel)TempData[TEMP_DATA_MODEL_KEY];

            return View(GetConfiguredEmployeeModel(account, model, adGetParam));
        }


        private EmployeeHomePageModel GetConfiguredEmployeeModel(Account account, EmployeeHomePageModel model, AdvancedGetParameters adGetParam)
        {
            if (model == null)
            {
                model = new EmployeeHomePageModel();
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

            //

            //var announcements = annAccessor.AnnouncementManagerHelper.AdvancedGetAnnouncementsAsList(adGetParam);

            var selfAccCategories = accToEmployeeDivCatAccessor.EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(model.LoggedInAccount.Id, new AdvancedGetParameters());
            Category selfAccCat = null;
            foreach (int empDivCatId in selfAccCategories)
            {
                selfAccCat = employeeDivCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromId(empDivCatId);
                break;
            }


            Func<int, int, bool> annRelCondition = (primaryId, targetId) =>
            {
                return selfAccCat != null && selfAccCat.Id == targetId;
            };

            var announcementRelations = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsSatisfyingCondition(adGetParam, annRelCondition, false);

            var announcements = new List<Announcement>();
            foreach(Relation rel in announcementRelations)
            {
                announcements.Add(annAccessor.AnnouncementManagerHelper.GetAnnouncementInfoFromId(rel.PrimaryId));
            }

            //


            if (announcements != null) {
                model.EntitiesInPage = announcements;
            }

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

            model.TotalEntityCount = annToCatAccessor.EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationCountSatisfyingCondition(countAdGetParam, annRelCondition, false);

            if (!model.IsPageIndexValid(model.CurrentPageIndex))
            {
                model.CurrentPageIndex = 1;
            }

            return model;
        }

        
        [HttpPost]
        [ActionName(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE__EXECUTE_ACTION)]
        public ActionResult EmployeeHomePage_ExecuteAction(EmployeeHomePageModel model, string executeAction)
        {
            if (executeAction.Equals("GoToEmployeeRecord"))
            {
                return GoTo_EditSelfEmployeeRecords(); //located in base controller
            }
            else if (DomIdentifier.IsDomIdentifier(executeAction))
            {
                var domElements = DomIdentifier.GetElementsOfDomIdentifier(executeAction);

                if (domElements[0].Equals(AdminController.DOM_IDENTIFIER_TYPE_SINGLE_ANNOUNCEMENT))
                {
                    var annId = int.Parse(domElements[1]);

                    return EmployeeHomePage_TransitionToSingleAnnouncementPage(annId);
                }
                else if (domElements[0].Equals(AdminController.DOM_IDENFIFIER_TYPE_ANNOUNCEMENT_PAGE_IN_INDEX))
                {
                    var pageIndex = int.Parse(domElements[1]);

                    return EmployeeHomePage_DoPageIndexChange_OfAnnouncement(model, pageIndex);
                }
            }
            else if (executeAction.Equals("TextFilterSubmit"))
            {
                return SetTextFilter_OfHomePageModelModel_ThenGoToHomePage(model);
            }
            

            return RedirectToAction(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE, ControllerNameConstants.EMPLOYEE_CONTROLLER_NAME);
            
        }


        private ActionResult EmployeeHomePage_DoPageIndexChange_OfAnnouncement(EmployeeHomePageModel model, int pageIndex)
        {
            model.CurrentPageIndex = pageIndex;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE, ControllerNameConstants.EMPLOYEE_CONTROLLER_NAME);
        }

        private ActionResult EmployeeHomePage_TransitionToSingleAnnouncementPage(int annId)
        {
            TempData.Add(GenericUserController.TEMP_DATA_ANNOUNCEMENT_ID_KEY, annId);

            return RedirectToAction(ActionNameConstants.GENERIC_USER__ANNOUNCEMENT_PAGE__GO_TO, ControllerNameConstants.GENERIC_USER_CONTROLLER_NAME);
        }

        private ActionResult SetTextFilter_OfHomePageModelModel_ThenGoToHomePage(EmployeeHomePageModel model)
        {
            model.CurrentPageIndex = 1;

            TempData.Add(TEMP_DATA_MODEL_KEY, model);

            return RedirectToAction(ActionNameConstants.EMPLOYEE_SIDE__HOME_PAGE, ControllerNameConstants.EMPLOYEE_CONTROLLER_NAME);
        }


        #endregion


    }

}