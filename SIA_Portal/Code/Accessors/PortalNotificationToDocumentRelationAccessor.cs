using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.RelationManager.Config;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Accessors
{
    public class PortalNotificationToDocumentRelationAccessor
    {
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "NotificationToDocumentRelationTable";

        
        const string NOTIF_ID_COL_NAME = "NotifId";
        const string DOCU_FILE_ID_COL_NAME = "DocumentFileId";


        public RelationDatabasePathConfig EntityToCategoryDatabasePathConfig { get; }

        public RelationDatabaseManagerHelper EntityToCategoryDatabaseManagerHelper { get; }


        public PortalNotificationToDocumentRelationAccessor()
        {
            EntityToCategoryDatabasePathConfig = new RelationDatabasePathConfig(DATABASE_CONN_STRING, NOTIF_ID_COL_NAME, DOCU_FILE_ID_COL_NAME, TABLE_NAME);

            EntityToCategoryDatabaseManagerHelper = new RelationDatabaseManagerHelper(EntityToCategoryDatabasePathConfig);
        }


    }
}