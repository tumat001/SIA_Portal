using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.NotificationManager.Config;
using CommonDatabaseActionReusables.NotificationManager;

namespace SIA_Portal.Accessors
{
    public class PortalNotificationAccessor
    {
        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "NotificationTable";

        const string ID_COLUMN_NAME = "Id";
        const string TITLE_COLUMN_NAME = "Title";
        const string CONTENT_COLUMN_NAME = "Content";
        const string IS_HIGHLIGHTED_COLUMN_NAME = "IsHighlighted";
        const string DATE_TIME_SENT_COLUMN_NAME = "DateSent";


        public NotificationDatabasePathConfig NotificationDatabasePathConfig { get; }

        public NotificationDatabaseManagerHelper NotificationManagerHelper { get; }

        public PortalNotificationAccessor()
        {
            NotificationDatabasePathConfig = new NotificationDatabasePathConfig(DATABASE_CONN_STRING, ID_COLUMN_NAME,
                TITLE_COLUMN_NAME, CONTENT_COLUMN_NAME, IS_HIGHLIGHTED_COLUMN_NAME, DATE_TIME_SENT_COLUMN_NAME,
               TABLE_NAME);

            NotificationManagerHelper = new NotificationDatabaseManagerHelper(NotificationDatabasePathConfig);
        }

    }
}