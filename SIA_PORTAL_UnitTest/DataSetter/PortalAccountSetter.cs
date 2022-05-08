using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.Accessors;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using CommonDatabaseActionReusables.BooleanCorrManager;
using CommonDatabaseActionReusables.PermissionsManager;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_PORTAL_UnitTest.DataSetter
{

    [TestClass]
    public class PortalAccountSetter
    {

        private static PortalAccountAccessor portalAccAccessor;
        private static PortalAccountMustChangeCredentialsAccessor accMustChangeCredentialsAccessor;
        private static PortalPermissionAccessor portalPermissionAccessor;
        private static PortalAccountToPermissionsAccessor portalAccToPermissionAccessor;
        private static PortalAccountToAccDivCatAccessor portalAccountToAccDivCatAccessor;
        private static PortalAccountDivisionCategoryAccessor portalAccountDivisionCategoryAccessor;
        private static PortalAccountToDivisionResponsibilityAccessor portalAccountToDivisionResponsibilityAccessor;

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalAccAccessor = new PortalAccountAccessor();
            accMustChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();
            portalPermissionAccessor = new PortalPermissionAccessor();
            portalAccToPermissionAccessor = new PortalAccountToPermissionsAccessor();
            portalAccountToAccDivCatAccessor = new PortalAccountToAccDivCatAccessor();
            portalAccountDivisionCategoryAccessor = new PortalAccountDivisionCategoryAccessor();
            portalAccountToDivisionResponsibilityAccessor = new PortalAccountToDivisionResponsibilityAccessor();
        }



        //

        [TestCategory("Creating Accounts")]
        [TestMethod]
        public void Creating_Accounts()
        {

            var adminBuilder01 = new Account.Builder();
            adminBuilder01.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder01.DisabledFromLogIn = false;
            adminBuilder01.Email = "sampleAdminEmail@gmail.com";
            adminBuilder01.Username = "Admin001";

            var adAccId01 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder01, "AdminPassword01");

            /*
            var boolManager01 = new BooleanCorrelation.Builder();
            boolManager01.OwnerId = adAccId01;
            boolManager01.BoolValue = true;
            accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.CreateBooleanCorr(boolManager01);
            */

            //

            var adminBuilder02 = new Account.Builder();
            adminBuilder02.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder02.DisabledFromLogIn = false;
            adminBuilder02.Email = "sampleAdminEmail02@gmail.com";
            adminBuilder02.Username = "Admin002";

            var adAccId02 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder02, "AdminPassword02");

            //

            var adminBuilder03 = new Account.Builder();
            adminBuilder03.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder03.DisabledFromLogIn = false;
            adminBuilder03.Email = "sampleAdminEmail03@gmail.com";
            adminBuilder03.Username = "None003";

            var adAccId03 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder03, "AdminPassword03");

            //

            var adminBuilder04 = new Account.Builder();
            adminBuilder04.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder04.DisabledFromLogIn = false;
            adminBuilder04.Email = "sampleAdminEmail04@gmail.com";
            adminBuilder04.Username = "Faculty004";

            var adAccId04 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder04, "AdminPassword04");

            //

            var adminBuilder05 = new Account.Builder();
            adminBuilder05.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder05.DisabledFromLogIn = false;
            adminBuilder05.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder05.Username = "FrontDesk005";

            var adAccId05 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder05, "AdminPassword05");

            //

            var adminBuilder06 = new Account.Builder();
            adminBuilder06.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder06.DisabledFromLogIn = false;
            adminBuilder06.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder06.Username = "HResources006";

            var adAccId06 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder06, "AdminPassword06");

            //

            var adminBuilder07 = new Account.Builder();
            adminBuilder07.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder07.DisabledFromLogIn = false;
            adminBuilder07.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder07.Username = "HResources&Faculty007";

            var adAccId07 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder07, "AdminPassword07");

            //

            var adminBuilder08 = new Account.Builder();
            adminBuilder08.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder08.DisabledFromLogIn = false;
            adminBuilder08.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder08.Username = "HResources&FrontD008";

            var adAccId08 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder08, "AdminPassword08");

            //

            var adminBuilder09 = new Account.Builder();
            adminBuilder09.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder09.DisabledFromLogIn = false;
            adminBuilder09.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder09.Username = "FrontD&Faculty009";

            var adAccId09 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder09, "AdminPassword09");

            //

            var adminBuilder10 = new Account.Builder();
            adminBuilder10.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder10.DisabledFromLogIn = false;
            adminBuilder10.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder10.Username = "FrontD&Faculty&HR010";

            var adAccId10 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder10, "AdminPassword10");

            //


            // PERMISSIONS

            portalAccToPermissionAccessor.EntityToCategoryDatabaseManagerHelper.DeleteAllRelations();

            var allPermissions = portalPermissionAccessor.PermissionDatabaseManagerHelper.AdvancedGetPermissionsAsList(new AdvancedGetParameters());
            foreach (Permission perm in allPermissions)
            {
                portalAccToPermissionAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId01, perm.Id);
                portalAccToPermissionAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId02, perm.Id);
            }


            // DIVISIONS

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.DeleteAllRelations();

            var HRDivisionName = PortalAccountDivisionCategorySetter.DIVISION_HR;
            var HRDivision = portalAccountDivisionCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromName(HRDivisionName);

            var FDDivisionName = PortalAccountDivisionCategorySetter.DIVISION_FRONT_DESK;
            var FDDivision = portalAccountDivisionCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromName(FDDivisionName);

            var FacultyDivisionName = PortalAccountDivisionCategorySetter.DIVISION_FACULTY;
            var FacultyDivision = portalAccountDivisionCategoryAccessor.CategoryDatabaseManagerHelper.GetCategoryInfoFromName(FacultyDivisionName);


            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId01, HRDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId02, HRDivision.Id);

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId04, FacultyDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId05, FDDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId06, HRDivision.Id);

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId07, HRDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId07, FacultyDivision.Id);

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId08, HRDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId08, FDDivision.Id);

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId09, FacultyDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId09, FDDivision.Id);

            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId10, FacultyDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId10, FDDivision.Id);
            portalAccountToAccDivCatAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId10, HRDivision.Id);
            

            // DIV Respo

            portalAccountToDivisionResponsibilityAccessor.EntityToCategoryDatabaseManagerHelper.DeleteAllRelations();

            portalAccountToDivisionResponsibilityAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId01, FDDivision.Id);
            portalAccountToDivisionResponsibilityAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId01, HRDivision.Id);
            portalAccountToDivisionResponsibilityAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId01, FacultyDivision.Id);
            
            portalAccountToDivisionResponsibilityAccessor.EntityToCategoryDatabaseManagerHelper.CreatePrimaryToTargetRelation(adAccId02, HRDivision.Id);

        }


        //

        [TestCategory("Deleting Accounts")]
        [TestMethod]
        public void Deleting_All_Account()
        {
            portalAccAccessor.AccountDatabaseManagerHelper.DeleteAllAccounts();
        }

        //

        [TestCategory("Reseting Accounts")]
        [TestMethod]
        public void Clean_ResetOfAccounts()
        {
            portalAccAccessor.AccountDatabaseManagerHelper.DeleteAllAccounts();

            Creating_Accounts();
        }

    }
}
