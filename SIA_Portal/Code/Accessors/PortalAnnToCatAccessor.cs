using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDatabaseActionReusables.RelationManager.Config;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Accessors
{
    public class PortalAnnToCatAccessor
    {

        const string ANN_TO_CAT_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string ANN_TO_CAT_TABLE_NAME = "AnnouncementToCategoryRelationTable";

        const string ANN_ID_COL_NAME = "AnnouncementId";
        const string CAT_ID_COL_NAME = "CategoryId";


        public RelationDatabasePathConfig EntityToCategoryDatabasePathConfig { get; }

        public RelationDatabaseManagerHelper EntityToCategoryDatabaseManagerHelper { get; }


        public PortalAnnToCatAccessor()
        {
            EntityToCategoryDatabasePathConfig = new RelationDatabasePathConfig(ANN_TO_CAT_DATABASE_CONN_STRING, ANN_ID_COL_NAME, CAT_ID_COL_NAME, ANN_TO_CAT_TABLE_NAME);

            EntityToCategoryDatabaseManagerHelper = new RelationDatabaseManagerHelper(EntityToCategoryDatabasePathConfig);
        }


    }
}
