using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.BooleanCorrManager.Config;
using CommonDatabaseActionReusables.BooleanCorrManager;

namespace SIA_Portal.Accessors
{
    public class PortalAccountMustChangeCredentialsAccessor
    {

        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "AccountMustChangeCredentialsTable";

        const string OWNER_ID_COL_NAME = "Id";
        const string BOOL_VALUE_COL_NAME = "BoolVal";

        public BooleanCorrDatabasePathConfig BooleanDatabasePathConfig { get; }

        public BooleanCorrDatabaseManagerHelper BooleanDatabaseManagerHelper { get; }


        public PortalAccountMustChangeCredentialsAccessor()
        {
            BooleanDatabasePathConfig = new BooleanCorrDatabasePathConfig(DATABASE_CONN_STRING, OWNER_ID_COL_NAME,
                BOOL_VALUE_COL_NAME, TABLE_NAME);

            BooleanDatabaseManagerHelper = new BooleanCorrDatabaseManagerHelper(BooleanDatabasePathConfig);
        }


    }
}