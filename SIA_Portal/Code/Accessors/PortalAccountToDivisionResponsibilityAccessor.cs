using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.RelationManager.Config;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Accessors
{
    public class PortalAccountToDivisionResponsibilityAccessor
    {

        const string ACC_TO_CAT_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string ACC_TO_CAT_TABLE_NAME = "AccountToDivisionResponsibilityTable";

        const string ACC_ID_COL_NAME = "AccountId";
        const string CAT_ID_COL_NAME = "AccDivId";


        public RelationDatabasePathConfig EntityToCategoryDatabasePathConfig { get; }

        public RelationDatabaseManagerHelper EntityToCategoryDatabaseManagerHelper { get; }


        public PortalAccountToDivisionResponsibilityAccessor()
        {
            EntityToCategoryDatabasePathConfig = new RelationDatabasePathConfig(ACC_TO_CAT_DATABASE_CONN_STRING, ACC_ID_COL_NAME, CAT_ID_COL_NAME, ACC_TO_CAT_TABLE_NAME);

            EntityToCategoryDatabaseManagerHelper = new RelationDatabaseManagerHelper(EntityToCategoryDatabasePathConfig);
        }

    }
}