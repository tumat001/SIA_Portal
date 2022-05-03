using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDatabaseActionReusables.ImageManager.Actions;
using CommonDatabaseActionReusables.ImageManager.Configs;
using CommonDatabaseActionReusables.ImageManager;

namespace SIA_Portal.Accessors
{
    public class PortalImageAccessor
    {

        const string IMAGE_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string IMAGE_TABLE_NAME = "ImageTable";

        const string IMAGE_ID_COL_NAME = "Id";
        const string IMAGE_DATA_TYPE_COL_NAME = "ImageDataType";
        const string IMAGE_STORE_TYPE_COL_NAME = "ImageStoreType";
        const string IMAGE_BYTES_COL_NAME = "ImageBytes";
        const string IMAGE_FULL_PATH_COL_NAME = "ImageFullFilePath";

        public ImageDatabasePathConfig ImageDatabasePathConfig { get; }

        public ImageDatabaseManagerHelper ImageDatabaseManagerHelper { get; }

        public PortalImageAccessor()
        {
            ImageDatabasePathConfig = new ImageDatabasePathConfig(IMAGE_DATABASE_CONN_STRING,
                IMAGE_ID_COL_NAME, IMAGE_DATA_TYPE_COL_NAME, IMAGE_STORE_TYPE_COL_NAME, IMAGE_BYTES_COL_NAME, IMAGE_FULL_PATH_COL_NAME,
                IMAGE_TABLE_NAME);

            ImageDatabaseManagerHelper = new ImageDatabaseManagerHelper(ImageDatabasePathConfig);
        }


    }
}
