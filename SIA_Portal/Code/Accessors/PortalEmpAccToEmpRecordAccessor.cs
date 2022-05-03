using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.RelationManager.Config;
using CommonDatabaseActionReusables.RelationManager;

namespace SIA_Portal.Accessors
{
    public class PortalEmpAccToEmpRecordAccessor
    {

        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string REL_TABLE_NAME = "EmployeeToEmployeeRecordDataRelationTable";

        const string PRIMARY_ID_COL_NAME = "AccountId";
        const string TARGET_ID_COL_NAME = "EmployeeRecordId";


        public RelationDatabasePathConfig EntityToRecordDatabasePathConfig { get; }

        public RelationDatabaseManagerHelper EntityToRecordDatabaseManagerHelper { get; }


        public PortalEmpAccToEmpRecordAccessor()
        {
            EntityToRecordDatabasePathConfig = new RelationDatabasePathConfig(DATABASE_CONN_STRING, PRIMARY_ID_COL_NAME, TARGET_ID_COL_NAME, REL_TABLE_NAME);

            EntityToRecordDatabaseManagerHelper = new RelationDatabaseManagerHelper(EntityToRecordDatabasePathConfig);
        }


    }
}