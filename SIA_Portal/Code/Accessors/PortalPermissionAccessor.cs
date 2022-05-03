using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.PermissionsManager.Config;
using CommonDatabaseActionReusables.PermissionsManager;

namespace SIA_Portal.Accessors
{
    public class PortalPermissionAccessor
    {

        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "PermissionsTable";

        const string PERMISSION_ID_COL_NAME = "Id";
        const string PERMISSION_NAME_COL_NAME = "Name";
        const string PERMISSION_DESC_COL_NAME = "Description";

        public PermissionDatabasePathConfig PermissionDatabasePathConfig { get; }

        public PermissionDatabaseManagerHelper PermissionDatabaseManagerHelper { get; }


        public PortalPermissionAccessor()
        {
            PermissionDatabasePathConfig = new PermissionDatabasePathConfig(DATABASE_CONN_STRING, PERMISSION_ID_COL_NAME, PERMISSION_NAME_COL_NAME,
                PERMISSION_DESC_COL_NAME, TABLE_NAME);

            PermissionDatabaseManagerHelper = new PermissionDatabaseManagerHelper(PermissionDatabasePathConfig);
        }

    }
}