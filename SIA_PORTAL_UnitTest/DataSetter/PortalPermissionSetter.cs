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
using CommonDatabaseActionReusables.PermissionsManager;

namespace SIA_PORTAL_UnitTest.DataSetter
{

    [TestClass]
    public class PortalPermissionSetter
    {

        private static PortalPermissionAccessor portalPermissionAccessor;


        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalPermissionAccessor = new PortalPermissionAccessor();
        }

        //

        [TestCategory("Reset")]
        [TestMethod]
        public void CleanReset_Permissions()
        {
            DeleteAll_Permissions();
            Create_Permissions();
        }



        //

        [TestCategory("Create")]
        [TestMethod]
        public void Create_Permissions()
        {
            /*
            var builder01 = new Permission.Builder();
            builder01.Name = PermissionConstants.MANAGE_EMPLOYEE_ACCOUNTS;
            builder01.SetDescriptionUsingString("Admins with this permission can view, create, edit and delete employee accounts.");
            int id01 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder01);
            */

            var builder02 = new Permission.Builder();
            builder02.Name = PermissionConstants.MANAGE_ACCOUNTS;
            builder02.SetDescriptionUsingString("Accounts with this permission can view, create, edit and delete accounts within their divisional responsibilities. This does not include the account's permissions and divisional responsibilities. The permitted cannot edit their own account via this method.");
            int id02 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder02);

            var builder03 = new Permission.Builder();
            builder03.Name = PermissionConstants.MANAGE_ACCOUNT_PERMISSIONS;
            builder03.SetDescriptionUsingString("Accounts with this permission can view permissions, and edit accounts's permissions. The permitted cannot edit their own permissions.");
            int id03 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder03);

            var builder04 = new Permission.Builder();
            builder04.Name = PermissionConstants.MANAGE_DIVISIONS;
            builder04.SetDescriptionUsingString("Accounts with this permission can view, create, edit and delete divisions (category of accounts).");
            int id04 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder04);
            
            var builder05 = new Permission.Builder();
            builder05.Name = PermissionConstants.MANAGE_DIVISIONAL_RESPONSIBILITIES;
            builder05.SetDescriptionUsingString("Accounts with this permission can view divisions (category of accounts), and edit account's divisional responsibility.");
            int id05 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder05);




            //
            var builder10 = new Permission.Builder();
            builder10.Name = PermissionConstants.MANAGE_ANNOUNCEMENTS;
            builder10.SetDescriptionUsingString("Accounts with this permission can view, create, edit and delete announcements. Created announcements, and announcements visible to the permitted, can only be within the account's divisional responsibilities.");
            int id10 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder10);

            //
            var builder20 = new Permission.Builder();
            builder20.Name = PermissionConstants.MANAGE_REQUESTABLE_DOCUMENT;
            builder20.SetDescriptionUsingString("Accounts with this permission can view, create, edit and delete requestable documents.");
            int id20 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder20);

            var builder21 = new Permission.Builder();
            builder21.Name = PermissionConstants.MANAGE_QUEUE_FOR_REQUESTABLE_DOCUMENTS;
            builder21.SetDescriptionUsingString("Accounts with this permission can view the queue for requests of requestable documents, and resolve requests. Accounts can also view requestable documents.");
            int id21 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder21);

            var builder22 = new Permission.Builder();
            builder22.Name = PermissionConstants.MANAGE_HISTORY_OF_REQUESTED_DOCUMENT;
            builder22.SetDescriptionUsingString("Accounts with this permission can view completed and terminated requests for documents. Accounts can also view requestable documents.");
            int id22 = portalPermissionAccessor.PermissionDatabaseManagerHelper.CreatePermission(builder22);


            //

        }

        //

        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteAll_Permissions()
        {
            portalPermissionAccessor.PermissionDatabaseManagerHelper.DeleteAllPermissions();
            
        }
    }
}
