using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.DocumentManager.Config;
using CommonDatabaseActionReusables.DocumentManager;

namespace SIA_Portal.Accessors
{
    public class PortalDocumentFileAccessor
    {


        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "DocumentFileTable";

        const string DOCUMENT_ID_COL_NAME = "Id";
        const string DOCUMENT_PATH_COL_NAME = "DocuFullPath";
        const string DOCUMENT_ORIGINAL_NAME_COL_NAME = "DocuOriginalName";
        const string DOCUMENT_EXT_COL_NAME = "DocuExt";

        //TODO Finish this

        public DocumentDatabasePathConfig DocumentDatabasePathConfig { get; }

        public DocumentDatabaseManagerHelper DocumentDatabaseManagerHelper { get; }


        public PortalDocumentFileAccessor()
        {
            DocumentDatabasePathConfig = new DocumentDatabasePathConfig(DATABASE_CONN_STRING, DOCUMENT_ID_COL_NAME, DOCUMENT_PATH_COL_NAME,
                DOCUMENT_ORIGINAL_NAME_COL_NAME, DOCUMENT_EXT_COL_NAME, TABLE_NAME);

            DocumentDatabaseManagerHelper = new DocumentDatabaseManagerHelper(DocumentDatabasePathConfig);
        }

    }
}