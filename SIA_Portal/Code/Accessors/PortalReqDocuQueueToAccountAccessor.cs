using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.RelationManager.Config;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Accessors
{
    public class PortalReqDocuQueueToAccountAccessor
    {

        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "ReqDocuQueueToAccTable";

        const string REQ_DOCU_QUEUE_ID_COL_NAME = "ReqDocuQueueId";
        const string ACC_ID_COL_NAME = "AccountId";


        public RelationDatabasePathConfig EntityToCategoryDatabasePathConfig { get; }

        public RelationDatabaseManagerHelper EntityToCategoryDatabaseManagerHelper { get; }


        public PortalReqDocuQueueToAccountAccessor()
        {
            EntityToCategoryDatabasePathConfig = new RelationDatabasePathConfig(DATABASE_CONN_STRING, REQ_DOCU_QUEUE_ID_COL_NAME, ACC_ID_COL_NAME, TABLE_NAME);

            EntityToCategoryDatabaseManagerHelper = new RelationDatabaseManagerHelper(EntityToCategoryDatabasePathConfig);
        }


    }
}