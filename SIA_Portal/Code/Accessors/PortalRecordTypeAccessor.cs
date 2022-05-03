using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDatabaseActionReusables.RecordTypeManager.Configs;
using CommonDatabaseActionReusables.RecordTypeManager;

namespace SIA_Portal.Accessors
{
    public class PortalRecordTypeAccessor
    {

        //Change these values upon changing database
        const string RECORD_TYPE_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string RECORD_TYPE_TABLE_NAME = "RecordTypeTable";

        const string RECORD_TYPE_ID_COL_NAME = "Id";
        const string RECORD_TYPE_NAME_COL_NAME = "Name";
        const string RECORD_TYPE_DATA_TYPE_COL_NAME = "RecordDataType";

        public RecordTypeDatabasePathConfig RecordTypeDatabasePathConfig { get; }

        public RecordTypeDatabaseManagerHelper RecordTypeManagerHelper { get; }


        public PortalRecordTypeAccessor()
        {
            RecordTypeDatabasePathConfig = new RecordTypeDatabasePathConfig(RECORD_TYPE_DATABASE_CONN_STRING,
                RECORD_TYPE_ID_COL_NAME, RECORD_TYPE_NAME_COL_NAME, RECORD_TYPE_DATA_TYPE_COL_NAME,
                RECORD_TYPE_TABLE_NAME);

            RecordTypeManagerHelper = new RecordTypeDatabaseManagerHelper(RecordTypeDatabasePathConfig);
        }


    }
}
