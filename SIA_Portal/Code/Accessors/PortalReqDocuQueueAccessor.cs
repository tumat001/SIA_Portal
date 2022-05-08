using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.QueueManager.Config;
using CommonDatabaseActionReusables.QueueManager;

namespace SIA_Portal.Accessors
{
    public class PortalReqDocuQueueAccessor
    {

        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "RequestDocumentQueueTable";

        const string QUEUE_ID_COL_NAME = "Id";
        const string QUEUE_DESC_COL_NAME = "Description";
        const string QUEUE_PRIORITY_LEVEL_COL_NAME = "PriorityLevel";
        const string QUEUE_STATUS_COL_NAME = "Status";
        const string QUEUE_STATUS_DESC_COL_NAME = "StatusDescription";
        const string QUEUE_DATE_TIME_COL_NAME = "DateTimeOfQueue";


        public QueueDatabasePathConfig QueueDatabasePathConfig { get; }

        public QueueDatabaseManagerHelper QueueDatabaseManagerHelper { get; }


        public PortalReqDocuQueueAccessor()
        {
            QueueDatabasePathConfig = new QueueDatabasePathConfig(DATABASE_CONN_STRING, QUEUE_ID_COL_NAME, QUEUE_DESC_COL_NAME,
                QUEUE_PRIORITY_LEVEL_COL_NAME, QUEUE_STATUS_COL_NAME, QUEUE_STATUS_DESC_COL_NAME, QUEUE_DATE_TIME_COL_NAME, TABLE_NAME);

            QueueDatabaseManagerHelper = new QueueDatabaseManagerHelper(QueueDatabasePathConfig);
        }



    }
}