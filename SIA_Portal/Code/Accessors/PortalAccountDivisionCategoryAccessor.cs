using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.CategoryManager.Config;
using CommonDatabaseActionReusables.CategoryManager;

namespace SIA_Portal.Accessors
{
    public class PortalAccountDivisionCategoryAccessor
    {


        const string CATEGORY_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string CATEGORY_TABLE_NAME = "AccountDivisionCategoryTable";

        const string CATEGORY_ID_COL_NAME = "Id";
        const string CATEGORY_NAME_COL_NAME = "Name";


        public CategoryDatabasePathConfig CategoryDatabasePathConfig { get; }

        public CategoryDatabaseManagerHelper CategoryDatabaseManagerHelper { get; }


        public PortalAccountDivisionCategoryAccessor()
        {
            CategoryDatabasePathConfig = new CategoryDatabasePathConfig(CATEGORY_DATABASE_CONN_STRING, CATEGORY_ID_COL_NAME, CATEGORY_NAME_COL_NAME, CATEGORY_TABLE_NAME);

            CategoryDatabaseManagerHelper = new CategoryDatabaseManagerHelper(CategoryDatabasePathConfig);
        }

    }
}