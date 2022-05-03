using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDatabaseActionReusables.RecordStoreManager.Configs;
using CommonDatabaseActionReusables.RecordStoreManager;

namespace SIA_Portal.Accessors
{
    public class PortalRecordStoreAccessor
    {

        //Change these values upon changing database
        const string RECORD_STORE_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string RECORD_STORE_TABLE_NAME = "RecordDataTable";

        const string RECORD_STORE_ID_COL_NAME = "Id";
        const string RECORD_STORE_RECORD_DATA_COL_NAME = "RecordData";


        public RecordStoreDatabasePathConfig RecordStoreDatabasePathConfig { get; }

        public RecordStoreDatabaseManagerHelper RecordStoreDatabaseManagerHelper { get; }


        public PortalRecordStoreAccessor()
        {
            RecordStoreDatabasePathConfig = new RecordStoreDatabasePathConfig(RECORD_STORE_DATABASE_CONN_STRING,
                RECORD_STORE_ID_COL_NAME, RECORD_STORE_RECORD_DATA_COL_NAME,
                RECORD_STORE_TABLE_NAME);

            RecordStoreDatabaseManagerHelper = new RecordStoreDatabaseManagerHelper(RecordStoreDatabasePathConfig);
        }

    }
}
