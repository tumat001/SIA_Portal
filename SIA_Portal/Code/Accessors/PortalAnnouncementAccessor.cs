using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using CommonDatabaseActionReusables.AnnouncementManager;

namespace SIA_Portal.Accessors
{
    public class PortalAnnouncementAccessor
    {

        //Change these values upon changing database
        const string ANNOUNCEMENT_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string ANNOUNCEMENT_TABLE_NAME = "AnnouncementTable";

        const string ANNOUNCMENT_ID_COLUMN_NAME = "Id";
        const string ANNOUNCEMENT_TITLE_COLUMN_NAME = "Title";
        const string ANNOUNCEMENT_CONTENT_COLUMN_NAME = "Content";
        const string ANNOUNCEMENT_MAIN_IMAGE_ID_COLUMN_NAME = "MainImageId";
        const string ANNOUNCEMENT_DATE_TIME_CREATED_COLUMN_NAME = "DateTimeCreated";
        const string ANNOUNCEMENT_DATE_TIME_LAST_MODIFIED_COLUMN_NAME = "DateTimeLastModified";


        public AnnouncementDatabasePathConfig AnnouncementDatabasePathConfig { get; }

        public AnnouncementDatabaseManagerHelper AnnouncementManagerHelper { get; }

        public PortalAnnouncementAccessor()
        {
            AnnouncementDatabasePathConfig = new AnnouncementDatabasePathConfig(ANNOUNCEMENT_DATABASE_CONN_STRING, ANNOUNCMENT_ID_COLUMN_NAME,
                ANNOUNCEMENT_TITLE_COLUMN_NAME, ANNOUNCEMENT_CONTENT_COLUMN_NAME, ANNOUNCEMENT_MAIN_IMAGE_ID_COLUMN_NAME, ANNOUNCEMENT_DATE_TIME_CREATED_COLUMN_NAME,
                ANNOUNCEMENT_DATE_TIME_LAST_MODIFIED_COLUMN_NAME, ANNOUNCEMENT_TABLE_NAME);

            AnnouncementManagerHelper = new AnnouncementDatabaseManagerHelper(AnnouncementDatabasePathConfig);
        }


    }
}