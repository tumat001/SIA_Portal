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

namespace SIA_PORTAL_UnitTest.DataSetter
{

    [TestClass]
    public class PortalAccountSetter
    {

        private static PortalAccountAccessor portalAccAccessor;
        private static PortalAccountMustChangeCredentialsAccessor accMustChangeCredentialsAccessor;


        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalAccAccessor = new PortalAccountAccessor();
            accMustChangeCredentialsAccessor = new PortalAccountMustChangeCredentialsAccessor();
        }



        //

        [TestCategory("Creating Accounts")]
        [TestMethod]
        public void Creating_Accounts()
        {
            var employeeBuilder01 = new Account.Builder();
            employeeBuilder01.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder01.DisabledFromLogIn = false;
            employeeBuilder01.Email = "sampleEmail@gmail.com";
            employeeBuilder01.Username = "Employee001";

            var accId = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder01, "EmployeePassword01");


            var employeeBuilder02 = new Account.Builder();
            employeeBuilder02.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder02.DisabledFromLogIn = false;
            employeeBuilder02.Email = "sampleEmail2@gmail.com";
            employeeBuilder02.Username = "Employee002";

            var accId02 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder02, "EtudentPassword02");


            var employeeBuilder03 = new Account.Builder();
            employeeBuilder03.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder03.DisabledFromLogIn = false;
            employeeBuilder03.Email = "sampleEmail3@gmail.com";
            employeeBuilder03.Username = "Employee003";

            var accId03 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder03, "EmployeePassword03");


            var employeeBuilder04 = new Account.Builder();
            employeeBuilder04.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder04.DisabledFromLogIn = false;
            employeeBuilder04.Email = "sampleEmail4@gmail.com";
            employeeBuilder04.Username = "Employee004";

            var accId04 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder04, "EmployeePassword04");


            var employeeBuilder05 = new Account.Builder();
            employeeBuilder05.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder05.DisabledFromLogIn = false;
            employeeBuilder05.Email = "sampleEmail5@gmail.com";
            employeeBuilder05.Username = "Employee005";

            var accId05 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder05, "EmployeePassword05");


            var employeeBuilder06 = new Account.Builder();
            employeeBuilder06.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            employeeBuilder06.DisabledFromLogIn = false;
            employeeBuilder06.Email = "sampleEmail6@gmail.com";
            employeeBuilder06.Username = "Employee006";

            var accId06 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(employeeBuilder06, "EmployeePassword06");



            //

            var adminBuilder01 = new Account.Builder();
            adminBuilder01.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder01.DisabledFromLogIn = false;
            adminBuilder01.Email = "sampleAdminEmail@gmail.com";
            adminBuilder01.Username = "Admin001";

            var adAccId01 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder01, "AdminPassword01");

            var boolManager01 = new BooleanCorrelation.Builder();
            boolManager01.OwnerId = adAccId01;
            boolManager01.BoolValue = true;
            accMustChangeCredentialsAccessor.BooleanDatabaseManagerHelper.CreateBooleanCorr(boolManager01);


            //

            var adminBuilder02 = new Account.Builder();
            adminBuilder02.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder02.DisabledFromLogIn = false;
            adminBuilder02.Email = "sampleAdminEmail02@gmail.com";
            adminBuilder02.Username = "Admin002";

            var adAccId02 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder02, "AdminPassword02");


            var adminBuilder03 = new Account.Builder();
            adminBuilder03.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder03.DisabledFromLogIn = false;
            adminBuilder03.Email = "sampleAdminEmail03@gmail.com";
            adminBuilder03.Username = "Admin003";

            var adAccId03 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder03, "AdminPassword03");


            var adminBuilder04 = new Account.Builder();
            adminBuilder04.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder04.DisabledFromLogIn = false;
            adminBuilder04.Email = "sampleAdminEmail04@gmail.com";
            adminBuilder04.Username = "Admin004";

            var adAccId04 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder04, "AdminPassword04");


            var adminBuilder05 = new Account.Builder();
            adminBuilder05.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder05.DisabledFromLogIn = false;
            adminBuilder05.Email = "sampleAdminEmail05@gmail.com";
            adminBuilder05.Username = "Admin005";

            var adAccId05 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder05, "AdminPassword05");


            var adminBuilder06 = new Account.Builder();
            adminBuilder06.AccountType = TypeConstants.ACCOUNT_TYPE_NORMAL;
            adminBuilder06.DisabledFromLogIn = false;
            adminBuilder06.Email = "sampleAdminEmail06@gmail.com";
            adminBuilder06.Username = "Admin006";

            var adAccId06 = portalAccAccessor.AccountDatabaseManagerHelper.CreateAccount(adminBuilder06, "AdminPassword06");


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
